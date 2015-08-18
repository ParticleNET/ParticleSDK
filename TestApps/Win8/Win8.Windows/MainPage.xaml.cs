using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Win8
{
	/// <summary>
	/// A basic page that provides characteristics common to most applications.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		public MainPage()
		{
			this.InitializeComponent();
			AuthControl.Authenticated += AuthControl_Authenticated;
		}

		private async void AuthControl_Authenticated(object sender, EventArgs e)
		{
			var results = await App.Cloud.GetDevicesAsync();
			if (!results.Success)
			{
				MessageDialog dialog = new MessageDialog(results.ErrorDescription ?? "", results.Error ?? "");
				await dialog.ShowAsync();
			}
			else
			{
				await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
				{
					DevicesList.DataContext = results.Data;
				});
			}
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			if (!App.Cloud.IsAuthenticated)
			{
				AuthControl.Show();
			}
		}
	}
}
