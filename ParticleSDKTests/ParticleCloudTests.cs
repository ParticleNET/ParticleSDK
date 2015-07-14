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
			using(var cloud = new ParticleCloud())
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
	}
}
