/*
Copyright 2016 ParticleNET

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
using Particle;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Windows.Web.Http;
#else
using System.Net;
using NUnit.Framework;
using System.Net.Http;
#endif

namespace ParticleSDKTests
{
#if NETFX_CORE
	[TestClass]
#else
	[TestFixture]
#endif
	public class ParticleCloudTests
	{
#if NETFX_CORE
		[TestMethod]
#else
		[Test]
#endif
		public async Task AuthenticationTestAsync()
		{
			using (var cloud = new ParticleCloud())
			{
				var exc = AssertHelper.Throws<ArgumentNullException>(() => { cloud.LoginWithUserAsync(null, "sadf").GetAwaiter().GetResult(); });
				Assert.AreEqual("username", exc.ParamName);
				exc = AssertHelper.Throws<ArgumentNullException>(() => { cloud.LoginWithUserAsync("test", null).GetAwaiter().GetResult(); });
				Assert.AreEqual("password", exc.ParamName);

				var results = await cloud.LoginWithUserAsync("test@test.com", "test123");
				Assert.IsFalse(results.Success, "User some how authenticated?");
				Assert.AreEqual("User credentials are invalid", results.ErrorDescription);

				results = await cloud.LoginWithUserAsync("test", "test");
				Assert.IsTrue(results.Success, "User did not authenticate");
			}
		}

#if NETFX_CORE
		[TestMethod]
#else
		[Test]
#endif
		public async Task AuthenticationAsyncHttpRequestExceptionTest()
		{
			using (var cloud = new ParticleCloud(new Uri("http://particletest.io")))
			{
				var result = await cloud.LoginWithUserAsync("test", "test");
				Assert.IsNotNull(result);
				Assert.IsFalse(result.Success);
#if NETFX_CORE
				Assert.AreEqual(@"The text associated with this error code could not be found.

The server name or address could not be resolved
", result.Error);
#else
				if(Environment.OSVersion.Platform == PlatformID.Unix)
				{
					Assert.AreEqual("Error: NameResolutionFailure", result.Error);

				}
				else
				{
					Assert.AreEqual("An error occurred while sending the request.", result.Error);
				}
#endif
			}
		}

#if NETFX_CORE
		[TestMethod]
#else
		[Test]
#endif
		public async Task MakeGetRequestAsyncTest()
		{
			using (var cloud = new ParticleCloud())
			{
				var exc = AssertHelper.Throws<ArgumentNullException>(() => { cloud.MakeGetRequestAsync(null).GetAwaiter().GetResult(); });
				Assert.AreEqual("method", exc.ParamName);
				AssertHelper.Throws<ParticleAuthenticationExeption>(() => { cloud.MakeGetRequestAsync("devices").GetAwaiter().GetResult(); });
				var stats = await cloud.LoginWithUserAsync("test", "test");
				Assert.IsTrue(stats.Success, "User did not authenticate");

				var results = await cloud.MakeGetRequestAsync("devices");
				Assert.AreEqual(
#if NETFX_CORE
					HttpStatusCode.Ok
#else
					HttpStatusCode.OK
#endif
					, results.StatusCode);
				var jrep = results.Response;
				Assert.AreEqual(JTokenType.Array, jrep.Type);
				JArray arr = (JArray)jrep;
				foreach (var obj in arr)
				{
					Assert.AreEqual(JTokenType.Object, obj.Type);
				}

			}
		}

#if NETFX_CORE
		[TestMethod]
#else
		[Test]
#endif
		public async Task RefreshToken_Test()
		{
			using (var cloud = new ParticleCloud())
			{
				var status = await cloud.LoginWithUserAsync("test", "test", 1);
				Assert.IsTrue(status.Success);
				await Task.Delay(2000);

				var req = await cloud.MakeGetRequestAsync("devices");
				Assert.AreEqual(HttpStatusCode.Unauthorized, req.StatusCode);

				status = await cloud.RefreshTokenAsync();
				Assert.IsTrue(status.Success);
			}
		}

#if NETFX_CORE
		[TestMethod]
#else
		[Test]
#endif
		public async Task GetDevicesAsyncTest()
		{
			using (var cloud = new ParticleCloudMock())
			{
				cloud.RequestCallBack = (t, m, p) =>
				{
					return new RequestResponse
					{
#if NETFX_CORE
						StatusCode = HttpStatusCode.Ok,
#else
						StatusCode = HttpStatusCode.OK,
#endif
						Response = JToken.Parse(@"[
						{
							""id"": ""1"",
							""name"": ""Work"",
							""last_app"": null,
							""last_ip_address"": ""192.168.0.1"",
							""last_heard"": ""2015-05-25T01:15:36.034Z"",
							""product_id"": 0,
							""connected"": false
						},
						{
							""id"": ""2"",
							""name"": ""Home"",
							""last_app"": null,
							""last_ip_address"": ""192.168.0.1"",
							""last_heard"": ""2015-05-25T01:15:59.188Z"",
							""product_id"": 0,
							""connected"": false
						},
						{
							""id"": ""3"",
							""name"": ""Proto"",
							""last_app"": null,
							""last_ip_address"": ""192.168.0.1"",
							""last_heard"": ""2015-07-24T00:37:07.820Z"",
							""product_id"": 6,
							""connected"": true
						}
]")
					};
				};

				var result = await cloud.GetDevicesAsync();
				Assert.IsNotNull(result);
				Assert.IsTrue(result.Success);
				Assert.IsNotNull(result.Data);
				var devices = result.Data;
				Assert.AreEqual(3, devices.Count);
				var device = devices[0];
				Assert.AreEqual("1", device.Id);
				Assert.AreEqual("Work", device.Name);
				device = devices[1];
				Assert.AreEqual("2", device.Id);
				Assert.AreEqual("Home", device.Name);
				device = devices[2];
				Assert.AreEqual("3", device.Id);
				Assert.AreEqual("Proto", device.Name);
			}
		}


#if NETFX_CORE
		[TestMethod]
#else
		[Test]
#endif
		public async Task GetDevicesAsyncHttpRequestExceptionTest()
		{
			using(var cloud = new ParticleCloudMock())
			{
				var ex = new Exception("Error connecting to the server");
				cloud.RequestCallBack = (t, m, p) =>
				{
					throw ex;
				};

				var result = await cloud.GetDevicesAsync();
				Assert.IsNotNull(result);
				Assert.IsFalse(result.Success);
				Assert.AreEqual(ex.Message, result.Error);
				Assert.AreEqual(ex, result.Exception);
			}
		}

#if NETFX_CORE
		[TestMethod]
#else
		[Test]
#endif
		public async Task SignupWithUserAsyncTest()
		{
			using (var cloud = new ParticleCloudMock())
			{
				cloud.RequestCallBack = (t, m, p) =>
				{
					return new RequestResponse
					{
#if NETFX_CORE
						StatusCode = HttpStatusCode.Ok,
#else
						StatusCode = HttpStatusCode.OK,
#endif
						Response = JToken.Parse(@"
{
	ok: false,
	errors:[""username must be an email address""]
}")
					};
				};

				ArgumentNullException e = AssertHelper.Throws<ArgumentNullException>(() => { cloud.SignupWithUserAsync(null, "test").GetAwaiter().GetResult(); });
				Assert.AreEqual("username", e.ParamName);
				e = AssertHelper.Throws<ArgumentNullException>(() => { cloud.SignupWithUserAsync("test", null).GetAwaiter().GetResult(); });
				Assert.AreEqual("password", e.ParamName);

				var result = await cloud.SignupWithUserAsync("test", "test");
				Assert.IsNotNull(result);
				Assert.IsFalse(result.Success);
				Assert.AreEqual("username must be an email address", result.Error);

				cloud.RequestCallBack = (t, m, p) =>
				{
					return new RequestResponse
					{
#if NETFX_CORE
						StatusCode = HttpStatusCode.Ok,
#else
						StatusCode = HttpStatusCode.OK,
#endif
						Response = JToken.Parse(@"
{
	ok: true
}")
					};
				};

				result = await cloud.SignupWithUserAsync("test@test.com", "test");
				Assert.IsNotNull(result);
				Assert.IsTrue(result.Success);
			}
		}

#if NETFX_CORE
		[TestMethod]
#else
		[Test]
#endif
		public async Task SignupWithUserAsyncHttpRequestExceptionTest()
		{
			using (var cloud = new ParticleCloudMock())
			{
				var ex = new Exception("Error Connecting to Server");
				cloud.RequestCallBack = (t, m, p) =>
				{
					throw ex;
				};

				var result = await cloud.SignupWithUserAsync("Test", "test");
				Assert.IsNotNull(result);
				Assert.IsFalse(result.Success);
				Assert.AreEqual(ex.Message, result.Error);
				Assert.AreEqual(ex, result.Exception);
			}
		}

#if NETFX_CORE
		[TestMethod]
#else
		[Test]
#endif
		public async Task RequestPasswordResetAsyncTest()
		{
			using (var cloud = new ParticleCloudMock())
			{
				cloud.RequestCallBack = (t, m, p) =>
				{
					return new RequestResponse
					{
						StatusCode = HttpStatusCode.NotFound,
						Response = JToken.Parse(@"
{
'ok': false,
'error': 'User not found.'
}")
					};
				};

				var ex = AssertHelper.Throws<ArgumentNullException>(() => { cloud.RequestPasswordResetAsync(null).GetAwaiter().GetResult(); });
				Assert.AreEqual("email", ex.ParamName);

				var result = await cloud.RequestPasswordResetAsync("test");
				Assert.IsNotNull(result);
				Assert.IsFalse(result.Success);
				Assert.AreEqual("User not found.", result.Error);

				cloud.RequestCallBack = (t, m, p) =>
				{
					return new RequestResponse
					{
#if NETFX_CORE
						StatusCode = HttpStatusCode.Ok,
#else
						StatusCode = HttpStatusCode.OK,
#endif
						Response = JToken.Parse(@"
{
'ok': true,
'message': 'Password reset email sent.'
}")
					};
				};

				result = await cloud.RequestPasswordResetAsync("test@test.com");
				Assert.IsNotNull(result);
				Assert.IsTrue(result.Success);
				Assert.AreEqual("Password reset email sent.", result.Message);
			}
		}

#if NETFX_CORE
		[TestMethod]
#else
		[Test]
#endif
		public async Task RequestPasswordResetAsyncHttpRequestExceptionTest()
		{
			using (var cloud = new ParticleCloudMock())
			{
				var ex = new Exception("Error connecting to server");
				cloud.RequestCallBack = (t, m, p) =>
				{
					throw ex;
				};
				var result = await cloud.RequestPasswordResetAsync("test");
				Assert.IsNotNull(result);
				Assert.IsFalse(result.Success);
				Assert.AreEqual(ex.Message, result.Error);
				Assert.AreEqual(ex, result.Exception);
			}
		}

#if NETFX_CORE
		[TestMethod]
#else
		[Test]
#endif
		public async Task ClaimDeviceAsyncTest()
		{
			using (var cloud = new ParticleCloudMock())
			{
				cloud.RequestCallBack = (t, m, p) =>
				{
					return new RequestResponse
					{
						StatusCode = HttpStatusCode.NotFound,
						Response = JToken.Parse(@"
{
ok: false,
errors: ['device does not exist']
}")
					};
				};

				var ex = AssertHelper.Throws<ArgumentNullException>(() => { cloud.ClaimDeviceAsync(null).GetAwaiter().GetResult(); });
				Assert.AreEqual("deviceId", ex.ParamName);

				var result = await cloud.ClaimDeviceAsync("123");
				Assert.IsNotNull(result);
				Assert.IsFalse(result.Success);
				Assert.AreEqual("device does not exist", result.Error);

				cloud.RequestCallBack = (t, m, p) =>
				{
					return new RequestResponse
					{
						StatusCode = HttpStatusCode.NotFound,
						Response = JToken.Parse(@"
{
  'user_id': '111111111111111111111111',
  'id': '222222222222222222222222',
  'connected': true,
  'ok': true
}")
					};
				};

				result = await cloud.ClaimDeviceAsync("222222222222222222222222");
				Assert.IsNotNull(result);
				Assert.IsTrue(result.Success);
			}
		}

#if NETFX_CORE
		[TestMethod]
#else
		[Test]
#endif
		public async Task ClaimDeviceAsyncHttpRequestExceptionTest()
		{
			using (var cloud = new ParticleCloudMock())
			{
				var ex = new Exception("Text HttpClient Request Exception");
				cloud.RequestCallBack = (t, m, p) =>
				{
					throw ex;
					//return new RequestResponse();
				};

				var result = await cloud.ClaimDeviceAsync("123");
				Assert.IsNotNull(result);
				Assert.IsFalse(result.Success);
				Assert.AreEqual(ex.Message, result.Error);
				Assert.AreEqual(ex, result.Exception);
			}
		}
	}
}
