using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Particle;
using Sannel.Helpers;
using System.Configuration;
using System.Threading.Tasks;

namespace ParticleSDKTests
{
	[TestClass]
	public class ParticleCloudTests
	{
		[TestMethod]
		public async Task AuthenticationTestAsync()
		{
			var cloud = new ParticleCloud();
			var exc = AssertHelpers.ThrowsException<ArgumentNullException>(() => { cloud.LoginWithUserAsync(null, "sadf").GetAwaiter().GetResult(); });
			Assert.AreEqual("username", exc.ParamName);
			exc = AssertHelpers.ThrowsException<ArgumentNullException>(() => { cloud.LoginWithUserAsync("test", null).GetAwaiter().GetResult(); });
			Assert.AreEqual("password", exc.ParamName);

			var results = await cloud.LoginWithUserAsync("test@test.com", "test123");
			Assert.IsFalse(results.IsAuthenticated, "User some how authenticated?");
			Assert.AreEqual("User credentials are invalid", results.Error);

			results = await cloud.LoginWithUserAsync(ConfigurationManager.AppSettings["Username"], ConfigurationManager.AppSettings["Password"]);
			Assert.IsTrue(results.IsAuthenticated, "User did not authenticate");
		}
	}
}
