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
using Particle;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Configuration;

namespace ParticleSDKTests
{
	[TestFixture]
	public class ParticleCloudTests
	{
		[Test]
		public async Task AuthenticationTestAsync()
		{
			using (var cloud = new ParticleCloud())
			{
				var exc = Assert.Throws<ArgumentNullException>(() => { cloud.LoginWithUserAsync(null, "sadf").GetAwaiter().GetResult(); });
				Assert.AreEqual("username", exc.ParamName);
				exc = Assert.Throws<ArgumentNullException>(() => { cloud.LoginWithUserAsync("test", null).GetAwaiter().GetResult(); });
				Assert.AreEqual("password", exc.ParamName);

				var results = await cloud.LoginWithUserAsync("test@test.com", "test123");
				Assert.IsFalse(results.Success, "User some how authenticated?");
				Assert.AreEqual("User credentials are invalid", results.ErrorDescription);

				results = await cloud.LoginWithUserAsync(ConfigurationManager.AppSettings["Username"], ConfigurationManager.AppSettings["Password"]);
				Assert.IsTrue(results.Success, "User did not authenticate");
			}
		}
	}
}
