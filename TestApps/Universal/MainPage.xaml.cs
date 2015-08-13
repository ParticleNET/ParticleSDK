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
		}

		protected async override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			if (!App.Cloud.IsAuthenticated)
			{
				AuthenticationDialog dialog = new AuthenticationDialog();
				var result = await dialog.ShowAsync();
				if(result == ContentDialogResult.Primary)
				{
					var dresults = await App.Cloud.GetDevicesAsync();
					if (dresults.Success)
					{
						DevicesComboBox.ItemsSource = dresults.Data;
						DevicesListView.ItemsSource = dresults.Data;
					}
				}
			}
		}

		private void DevicesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count > 0)
			{
				if (DevicesListView.SelectedItem != DevicesComboBox.SelectedItem)
				{
					DevicesComboBox.SelectedItem = DevicesListView.SelectedItem;
				}
			}
		}

		private void DevicesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if(e.AddedItems.Count > 0)
			{
				InternalFrame.Navigate(typeof(Common.Controls.DeviceInfo), e.AddedItems[0]);
				if(DevicesListView.SelectedItem != DevicesComboBox.SelectedItem)
				{
					DevicesListView.SelectedItem = DevicesComboBox.SelectedItem;
				}
			}
		}
	}
}
