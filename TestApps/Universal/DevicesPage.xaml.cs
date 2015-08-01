using Particle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Universal
{
	public sealed partial class DevicesPage : Page
	{
		public DevicesPage()
		{
			this.InitializeComponent();
		}

		protected async override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			try
			{
				var devices = await App.Cloud.GetDevicesAsync();
				DevicesList.ItemsSource = devices;
			}
			catch(ParticleException pe)
			{
				var dialog = new MessageDialog(pe.ErrorDescription);
				await dialog.ShowAsync();
			}
		}

		private void DevicesList_ItemClick(object sender, ItemClickEventArgs e)
		{
			var pd = e.ClickedItem as ParticleDevice;
			Frame.Navigate(typeof(DeviceInformationPage), pd);
		}

		private void DevicesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if(e.AddedItems.Count > 0)
			{
				var pd = e.AddedItems[0] as ParticleDevice;
				DevicesList.SelectedItem = 0;
				Frame.Navigate(typeof(DeviceInformationPage), pd);
			}
		}
	}
}
