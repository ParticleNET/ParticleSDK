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
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using NUnit.Framework;
#endif
using System;

namespace ParticleSDKTests
{
	public static class AssertHelper
	{
#if NETFX_CORE
		public static T Throws<T>(Action d)
			where T : Exception
		{
			return Assert.ThrowsException<T>(d);
		}
#else
		public static T Throws<T>(TestDelegate d)
			where T : Exception
		{
			return Assert.Throws<T>(d);
		}
#endif

		public static void IsInstanceOf<T>(Object obj)
		{
#if NETFX_CORE
			Assert.IsInstanceOfType(obj, typeof(T));
#else
			Assert.IsInstanceOf<T>(obj);
#endif
		}
	}
}
