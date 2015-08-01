using Particle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Win8;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Universal
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		public MainPage()
		{
			this.InitializeComponent();
			DataContext = AppSettings.Current;
		}

		private async void LoginAction_Tapped(object sender, TappedRoutedEventArgs e)
		{
			Authenticating.Visibility = Visibility.Visible;
			try
			{
				var result = await App.Cloud.LoginWithUserAsync(AppSettings.Current.Username, AppSettings.Current.Password);
				if (!result.Success)
				{
					MessageDialog dialog = new MessageDialog(result.ErrorDescription);
					await dialog.ShowAsync();
				}
				else
				{
					this.Frame.Navigate(typeof(DevicesPage));
				}
			}
			catch (ParticleException ex)
			{
				var dialog = new MessageDialog(ex.ToString());
				await dialog.ShowAsync();
			}

			Authenticating.Visibility = Visibility.Collapsed;
		}
	}
}
