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
	}
}
