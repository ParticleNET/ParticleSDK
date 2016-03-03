
using NUnit.Framework;
using Particle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ParticleSDKTests.NUnit
{
	[TestFixture]
	public class RHTests
	{
		private String GetString(String str)
		{
			var assm = typeof(ParticleCloud).GetTypeInfo().Assembly;
			var rh = assm.GetTypes().FirstOrDefault(i => i.Name == "RH");
			if(rh == null)
			{
				Assert.Fail("Unable to locate RH class");
			}

			var c = rh.GetProperties().FirstOrDefault(i => i.Name == "C");
			if(c == null)
			{
				Assert.Fail("Unable to locate c class");
			}

			var curr = c.GetValue(null);

			var m = rh.GetMethods().FirstOrDefault(i => i.Name == "GetString");
			if(m == null)
			{
				Assert.Fail("Unable to locate GetString method on class");
			}

			var ret = m.Invoke(curr, new String[] { str });
			Assert.IsInstanceOf(typeof(String), ret);
			return (String)ret;
		}
		[Test]
		public void GetStringTest()
		{
			var v = GetString("UnitTestString");
			Assert.AreEqual("This string is just for unit testing the ResourceHelper", v);
		}
	}
}
