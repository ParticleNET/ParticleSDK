using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if NETFX_CORE
using Windows.ApplicationModel.Resources;
#else
using System.Resources;
using System.Reflection;
#endif

namespace Particle
{
	/// <summary>
	/// A class to help get items located in the Resource files. This class is not meant to be used outside of the Particle SDK and is only left open so its easier to test with a unit test.
	/// </summary>
	internal class RH
	{
		/// <summary>
		/// Gets the current instance of the ResourceHelper
		/// </summary>
		/// <value>
		/// The current.
		/// </value>
		public static RH C { get; } = new RH();

#if NETFX_CORE
		private static ResourceLoader loader;
#else
		private static ResourceManager loader;
#endif

		/// <summary>
		/// Initializes a new instance of the <see cref="RH"/> class.
		/// </summary>
		private RH()
		{
#if NETFX_CORE
#if WINDOWS_APP_81
			var v = loader ?? (loader = ResourceLoader.GetForViewIndependentUse("Particle.Win8/Resources"));
#elif WINDOWS_PHONE_APP
			var v = loader ?? (loader = ResourceLoader.GetForViewIndependentUse("Particle.WP8/Resources"));
#elif WINDOWS_UWP
			var v = loader ?? (loader = ResourceLoader.GetForViewIndependentUse("Particle.UWP/Resources"));
#endif
#else
			var m = loader ?? (loader = new ResourceManager("Particle.Messages", typeof(RH).GetTypeInfo().Assembly));
#endif
		}

		/// <summary>
		/// Gets the string given the resource key
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public String GetString(String value)
		{
			return loader.GetString(value);
		}
	}
}
