
#if NETFX_CORE
using Windows.Web.Http;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using NUnit.Framework;
using System.Net;
#endif
using Particle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ParticleSDKTests.NUnit
{
#if NETFX_CORE
	[TestClass]
#else
	[TestFixture]
#endif
	public class RHTests
	{
		private String GetString(String str)
		{
			var assm = typeof(ParticleCloud).GetTypeInfo().Assembly;
#if NETFX_CORE
			var rh = assm.GetType("Particle.RH");
#else
			var rh = assm.GetTypes().FirstOrDefault(i => i.Name == "RH");
#endif
			if(rh == null)
			{
				Assert.Fail("Unable to locate RH class");
			}

#if NETFX_CORE
			var c = rh.GetRuntimeProperties().FirstOrDefault(i => i.Name == "C");
#else
			var c = rh.GetProperties().FirstOrDefault(i => i.Name == "C");
#endif
			if(c == null)
			{
				Assert.Fail("Unable to locate c class");
			}

			var curr = c.GetValue(null);

#if NETFX_CORE
			var m = rh.GetRuntimeMethods().FirstOrDefault(i => i.Name == "GetString");
#else
			var m = rh.GetMethods().FirstOrDefault(i => i.Name == "GetString");
#endif
			if(m == null)
			{
				Assert.Fail("Unable to locate GetString method on class");
			}

			var ret = m.Invoke(curr, new String[] { str });
			AssertHelper.IsInstanceOf<String>( ret);
			return (String)ret;
		}
#if NETFX_CORE
		[TestMethod]
#else
		[Test]
#endif
		public void GetStringTest()
		{
			var v = GetString("UnitTestString");
			Assert.AreEqual("This string is just for unit testing the ResourceHelper", v);
		}
	}
}
