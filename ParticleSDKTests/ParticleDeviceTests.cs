/*
Copyright 2015 ParticleNET

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

	http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
 */
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Sannel.Helpers;
using Particle;
using System.Threading.Tasks;

namespace ParticleSDKTests
{
	[TestClass]
	public class ParticleDeviceTests
	{

		[TestMethod]
		public void ParseTest()
		{
			var p = new ParticleDeviceMock(new JObject());
			var message = AssertHelpers.ThrowsException<ArgumentNullException>(() => { p.ParseObjectMock(null); });
			Assert.AreEqual("obj", message.ParamName);

			var obj = JObject.Parse(@"{'id':'3a', 'name':null}");
			p.ParseObjectMock(obj);
			Assert.AreEqual("3a", p.Id);
			Assert.AreEqual(null, p.Name);

			obj = JObject.Parse(@"{'id': '356a',
	'name': 'Work',
	'last_app': 'cheese',
	'last_ip_address': '192.168.0.1',
	'last_heard': '2015-05-25T01:15:36.034Z',
	'product_id': 0,
	'connected': true,
	'variables':{
		temp: 'double',
		temp2: 'int',
		temp3: 'string'
	},
	'functions':[
		'led',
		'led2'
	]}");
			p.ParseObjectMock(obj);
			Assert.AreEqual("356a", p.Id);
			Assert.AreEqual("Work", p.Name);
			Assert.AreEqual("cheese", p.LastApp);
			Assert.AreEqual("192.168.0.1", p.LastIPAddress);
			Assert.AreEqual(JToken.Parse("'2015-05-25T01:15:36.034Z'").Value<DateTime>(), p.LastHeard);
			Assert.AreEqual(ParticleDeviceType.SparkDeviceTypeCore, p.DeviceType);
			Assert.IsTrue(p.Connected);

			Assert.AreEqual(3, p.Variables.Count);
			var variable = p.Variables[0];
			Assert.AreEqual("temp", variable.Name);
			Assert.AreEqual("double", variable.Type);
			variable = p.Variables[1];
			Assert.AreEqual("temp2", variable.Name);
			Assert.AreEqual("int", variable.Type);
			variable = p.Variables[2];
			Assert.AreEqual("temp3", variable.Name);
			Assert.AreEqual("string", variable.Type);

			var functions = p.Functions;
			Assert.AreEqual(2, functions.Count);
			Assert.AreEqual("led", functions[0]);
			Assert.AreEqual("led2", functions[1]);
		}

		[TestMethod]
		public async Task RefreshAsyncTest()
		{
			ParticleCloudMock cloud = new ParticleCloudMock();
			cloud.RequestCallBack = (a, b, c) =>
			{
				Assert.AreEqual("GET", a);
				Assert.AreEqual("devices/3", b);
				return new RequestResponse
				{
					StatusCode = System.Net.HttpStatusCode.OK,
					Response = JToken.Parse(@"{'id': '3',
	'name': 'Work',
	'last_app': 'cheese',
	'last_ip_address': '192.168.0.1',
	'last_heard': '2015-05-25T01:15:36.034Z',
	'product_id': 0,
	'connected': true,
	'variables':{
		temp: 'double',
		temp2: 'int',
		temp3: 'string'
	},
	'functions':[
		'led',
		'led2'
	]}")
				};
			};

			var p = new ParticleDeviceMock(cloud, JObject.Parse("{'id':'3', 'name': 'test'}"));
			var result = await p.RefreshAsync();
			Assert.IsTrue(result.Success);
			Assert.AreEqual("3", p.Id);
			Assert.AreEqual("Work", p.Name);
			Assert.AreEqual("cheese", p.LastApp);
			Assert.AreEqual("192.168.0.1", p.LastIPAddress);
			Assert.AreEqual(JToken.Parse("'2015-05-25T01:15:36.034Z'").Value<DateTime>(), p.LastHeard);
			Assert.AreEqual(ParticleDeviceType.SparkDeviceTypeCore, p.DeviceType);
			Assert.IsTrue(p.Connected);

			Assert.AreEqual(3, p.Variables.Count);
			var variable = p.Variables[0];
			Assert.AreEqual("temp", variable.Name);
			Assert.AreEqual("double", variable.Type);
			variable = p.Variables[1];
			Assert.AreEqual("temp2", variable.Name);
			Assert.AreEqual("int", variable.Type);
			variable = p.Variables[2];
			Assert.AreEqual("temp3", variable.Name);
			Assert.AreEqual("string", variable.Type);

			var functions = p.Functions;
			Assert.AreEqual(2, functions.Count);
			Assert.AreEqual("led", functions[0]);
			Assert.AreEqual("led2", functions[1]);
		}

		[TestMethod]
		public async Task GetVariableValueAsyncTest()
		{
			ParticleCloudMock cloud = new ParticleCloudMock();
			cloud.RequestCallBack = (a, b, c) =>
			{
				Assert.AreEqual("GET", a);
				Assert.AreEqual("devices/3/temp", b);
				return new RequestResponse
				{
					StatusCode = System.Net.HttpStatusCode.OK,
					Response = JToken.Parse(@"{'name': 'temp',
  'result': 300,
  'coreInfo': {
						'name': 'weatherman',
						'deviceID': '0123456789abcdef01234567',
						'connected': true,
						'last_handshake_at': '2015-07-17T22:28:40.907Z',
						'last_app': ''
					}}")
				};
			};

			var p = new ParticleDeviceMock(cloud, JObject.Parse("{'id':'3', 'name': 'test', 'variables':{'temp':'int'}}"));

			var ex = AssertHelpers.ThrowsException<ArgumentNullException>(() => { p.GetVariableValueAsync((String)null).GetAwaiter().GetResult(); });
			Assert.AreEqual("variable", ex.ParamName);
			ex = AssertHelpers.ThrowsException<ArgumentNullException>(() => { p.GetVariableValueAsync((Variable)null).GetAwaiter().GetResult(); });
			Assert.AreEqual("variable", ex.ParamName);

			var results = await p.GetVariableValueAsync("temp");
			Assert.IsTrue(results.Success);
			Assert.IsNotNull(results.Data);
			var variable = results.Data;
			Assert.AreEqual("temp", variable.Name);
			Assert.AreEqual("300", variable.Value);

			cloud.RequestCallBack = (a, b, c) =>
			{
				Assert.AreEqual("GET", a);
				Assert.AreEqual("devices/3/temp", b);
				return new RequestResponse
				{
					StatusCode = System.Net.HttpStatusCode.OK,
					Response = JToken.Parse(@"{'name': 'temp',
  'result': 23,
  'coreInfo': {
						'name': 'weatherman',
						'deviceID': '0123456789abcdef01234567',
						'connected': true,
						'last_handshake_at': '2015-07-17T22:28:40.907Z',
						'last_app': ''
					}}")
				};
			};

			results = await p.GetVariableValueAsync(variable);
			Assert.IsTrue(results.Success);
			Assert.IsNotNull(results.Data);
			variable = results.Data;
			Assert.AreEqual("temp", variable.Name);
			Assert.AreEqual("23", variable.Value);
		}

		[TestMethod]
		public async Task CallFunctionAsyncTest()
		{
			using (ParticleCloudMock cloud = new ParticleCloudMock())
			{
				cloud.RequestCallBack = (a, b, c) =>
				{
					Assert.AreEqual("POST", a);
					Assert.AreEqual("devices/3/led", b);
					Assert.AreEqual(1, c.Count());
					var first = c.FirstOrDefault();
					Assert.IsNotNull(first);
					Assert.AreEqual("arg", first.Key);
					Assert.AreEqual("on", first.Value);
					return new RequestResponse
					{
						StatusCode = System.Net.HttpStatusCode.OK,
						Response = JToken.Parse(@"{
  'id': '3',
  'name': 'led',
  'last_app': '',
  'connected': true,
  'return_value': 1
}")
					};
				};

				var p = new ParticleDeviceMock(cloud, JObject.Parse("{'id':'3', 'name': 'test', 'functions':['led']}"));
				var exc = AssertHelpers.ThrowsException<ArgumentNullException>(() => { p.CallFunctionAsync(null, "").GetAwaiter().GetResult(); });
				Assert.AreEqual("functionName", exc.ParamName);
				var result = await p.CallFunctionAsync("led", "on");
				Assert.IsTrue(result.Success);
				Assert.AreEqual(1, result.Data);
			}
		}

		[TestMethod]
		public async Task UnclaimAsyncTest()
		{
			using (ParticleCloudMock cloud = new ParticleCloudMock())
			{
				cloud.RequestCallBack = (a, b, c) =>
				{
					Assert.AreEqual("DELETE", a);
					Assert.AreEqual("devices/3", b);
					return new RequestResponse
					{
						StatusCode = System.Net.HttpStatusCode.Forbidden,
						Response = JToken.Parse(@"{
		  'error': 'Permission Denied',
		  'info': 'I didn\'t recognize that device name or ID, try opening https://api.particle.io/v1/devices?access_token=...'
		}")
					};
				};

				var p = new ParticleDeviceMock(cloud, JObject.Parse("{'id':'3', 'name': 'test'}"));

				var result = await p.UnclaimAsync();
				Assert.IsNotNull(result);
				Assert.IsFalse(result.Success);
				Assert.AreEqual("Permission Denied", result.Error);
				Assert.AreEqual("I didn\'t recognize that device name or ID, try opening https://api.particle.io/v1/devices?access_token=...", result.Message);

				cloud.RequestCallBack = (a, b, c) =>
				{
					return new RequestResponse
					{
						StatusCode = System.Net.HttpStatusCode.OK,
						Response = JToken.Parse(@"{
	  'ok': true
	}")
					};
				};

				result = await p.UnclaimAsync();
				Assert.IsNotNull(result);
				Assert.IsTrue(result.Success);
			}
		}

		[TestMethod]
		public async Task RenameAsyncTest()
		{
			using (ParticleCloudMock cloud = new ParticleCloudMock())
			{
				cloud.RequestCallBack = (a, b, c) =>
				{
					Assert.AreEqual("PUT", a);
					Assert.AreEqual("devices/3", b);
					Assert.AreEqual(1, c.Length);
					var p1 = c[0];
					Assert.AreEqual("name", p1.Key);
					Assert.AreEqual("newTest", p1.Value);
					return new RequestResponse
					{
						StatusCode = System.Net.HttpStatusCode.OK,
						Response = JToken.Parse(@"{
		  'error': 'Nothing to do?'
		}")
					};
				};

				var p = new ParticleDeviceMock(cloud, JObject.Parse("{'id':'3', 'name': 'test'}"));

				var exc = AssertHelpers.ThrowsException<ArgumentNullException>(() => { p.RenameAsync(null).GetAwaiter().GetResult(); });
				Assert.AreEqual("newName", exc.ParamName);

				var result = await p.RenameAsync("newTest");
				Assert.IsNotNull(result);
				Assert.IsFalse(result.Success);
				Assert.AreEqual("Nothing to do?", result.Error);

				cloud.RequestCallBack = (a, b, c) =>
				{
					Assert.AreEqual("PUT", a);
					Assert.AreEqual("devices/3", b);
					Assert.AreEqual(1, c.Length);
					var p1 = c[0];
					Assert.AreEqual("name", p1.Key);
					Assert.AreEqual("newTest", p1.Value);
					return new RequestResponse
					{
						StatusCode = System.Net.HttpStatusCode.OK,
						Response = JToken.Parse(@"{
name: 'newTest',
id: '1234'
		}")
					};
				};

				result = await p.RenameAsync("newTest");
				Assert.IsNotNull(result);
				Assert.IsTrue(result.Success);
			}
		}
	}
}
