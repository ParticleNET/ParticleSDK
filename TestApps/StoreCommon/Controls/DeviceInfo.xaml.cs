using Particle;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Common.Controls
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public partial class DeviceInfo : Page
	{
		public DeviceInfo()
		{
			InitializeComponent();
		}

		protected async override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			var di = e.Parameter as ParticleDevice;
			if (di != null)
			{
				DataContext = di;
				Refreshing.Visibility = Windows.UI.Xaml.Visibility.Visible;
				var results = await di.RefreshAsync();
				if (!results.Success)
				{
					MessageDialog dialog = new MessageDialog(results.ErrorDescription, results.Error);
					await dialog.ShowAsync();
				}
				Refreshing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

			}
		}

		private async void Variable_Refresh_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{
			var variable = ((Button)sender).Tag as Variable;
			var result = await variable.RefreshValueAsync();
			if (!result.Success)
			{
				MessageDialog dialog = new MessageDialog(result.ErrorDescription, result.Error);
				await dialog.ShowAsync();
			}
		}
	}
}
