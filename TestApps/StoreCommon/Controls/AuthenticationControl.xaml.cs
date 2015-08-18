using Particle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Common.Controls
{
	public sealed partial class AuthenticationControl : UserControl
	{
		public AuthenticationControl()
		{
			this.InitializeComponent();
			DataContext = AppSettings.Current;
		}

		/// <summary>
		/// Returns true if the cloud has been authenticated to false otherwise
		/// </summary>
		/// <param name="cloud"></param>
		/// <returns></returns>
		public async Task<bool> AuthenticateAsync(ParticleCloud cloud)
		{
			ErrorContainer.Visibility = Visibility.Collapsed;
			LoggingInContainer.Visibility = Visibility.Visible;
			var result = await cloud.LoginWithUserAsync(AppSettings.Current.Username, AppSettings.Current.Password);
			if (!result.Success)
			{
				ErrorOutput.Text = result.Error ?? "";
				ErrorDescriptionOutput.Text = result.ErrorDescription ?? "";
				LoggingInContainer.Visibility = Visibility.Collapsed;
				ErrorContainer.Visibility = Visibility.Visible;
				return false;
			}
			LoggingInContainer.Visibility = Visibility.Collapsed;
			return true;
		}
	}
}
