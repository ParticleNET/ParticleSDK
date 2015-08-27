using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Particle;
using Sannel.Helpers;
using System.Configuration;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json.Linq;

namespace ParticleSDKTests
{
	[TestClass]
	public class ParticleCloudTests
	{
		[TestMethod]
		public async Task AuthenticationTestAsync()
		{
			using (var cloud = new ParticleCloud())
			{
				var exc = AssertHelpers.ThrowsException<ArgumentNullException>(() => { cloud.LoginWithUserAsync(null, "sadf").GetAwaiter().GetResult(); });
				Assert.AreEqual("username", exc.ParamName);
				exc = AssertHelpers.ThrowsException<ArgumentNullException>(() => { cloud.LoginWithUserAsync("test", null).GetAwaiter().GetResult(); });
				Assert.AreEqual("password", exc.ParamName);

				var results = await cloud.LoginWithUserAsync("test@test.com", "test123");
				Assert.IsFalse(results.Success, "User some how authenticated?");
				Assert.AreEqual("User credentials are invalid", results.ErrorDescription);

				results = await cloud.LoginWithUserAsync(ConfigurationManager.AppSettings["Username"], ConfigurationManager.AppSettings["Password"]);
				Assert.IsTrue(results.Success, "User did not authenticate");
			}
		}

		[TestMethod]
		public async Task MakeGetRequestAsyncTest()
		{
			using (var cloud = new ParticleCloud())
			{
				var exc = AssertHelpers.ThrowsException<ArgumentNullException>(() => { cloud.MakeGetRequestAsync(null).GetAwaiter().GetResult(); });
				Assert.AreEqual("method", exc.ParamName);
				AssertHelpers.ThrowsException<ParticleAuthenticationExeption>(() => { cloud.MakeGetRequestAsync("devices").GetAwaiter().GetResult(); });
				var stats = await cloud.LoginWithUserAsync(ConfigurationManager.AppSettings["Username"], ConfigurationManager.AppSettings["Password"]);
				Assert.IsTrue(stats.Success, "User did not authenticate");

				var results = await cloud.MakeGetRequestAsync("devices");
				Assert.AreEqual(HttpStatusCode.OK, results.StatusCode);
				var jrep = results.Response;
				Assert.AreEqual(JTokenType.Array, jrep.Type);
				JArray arr = (JArray)jrep;
				foreach (var obj in arr)
				{
					Assert.AreEqual(JTokenType.Object, obj.Type);
				}

			}
		}

		[TestMethod]
		public async Task RefreshToken_Test()
		{
			using (var cloud = new ParticleCloud())
			{
				var status = await cloud.LoginWithUserAsync(ConfigurationManager.AppSettings["Username"], ConfigurationManager.AppSettings["Password"], 1);
				Assert.IsTrue(status.Success);
				await Task.Delay(2000);

				var req = await cloud.MakeGetRequestAsync("devices");
				Assert.AreEqual(HttpStatusCode.Unauthorized, req.StatusCode);

				status = await cloud.RefreshTokenAsync();
				Assert.IsTrue(status.Success);
			}
		}

		[TestMethod]
		public async Task GetDevicesAsyncTest()
		{
			using (var cloud = new ParticleCloudMock())
			{
				cloud.RequestCallBack = (t, m, p) =>
				{
					return new RequestResponse
					{
						StatusCode = HttpStatusCode.OK,
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

		[TestMethod]
		public async Task SignupWithUserAsyncTest()
		{
			using (var cloud = new ParticleCloudMock())
			{
				cloud.RequestCallBack = (t, m, p) =>
				{
					return new RequestResponse
					{
						StatusCode = HttpStatusCode.OK,
						Response = JToken.Parse(@"
{
	ok: false,
	errors:[""username must be an email address""]
}")
					};
				};

				ArgumentNullException e = AssertHelpers.ThrowsException<ArgumentNullException>(() => { cloud.SignupWithUserAsync(null, "test").GetAwaiter().GetResult(); });
				Assert.AreEqual("username", e.ParamName);
				e = AssertHelpers.ThrowsException<ArgumentNullException>(() => { cloud.SignupWithUserAsync("test", null).GetAwaiter().GetResult(); });
				Assert.AreEqual("password", e.ParamName);

				var result = await cloud.SignupWithUserAsync("test", "test");
				Assert.IsNotNull(result);
				Assert.IsFalse(result.Success);
				Assert.AreEqual("Sign up error", result.Error);
				Assert.AreEqual("username must be an email address", result.ErrorDescription);

				cloud.RequestCallBack = (t, m, p) =>
				{
					return new RequestResponse
					{
						StatusCode = HttpStatusCode.OK,
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
	}
}
