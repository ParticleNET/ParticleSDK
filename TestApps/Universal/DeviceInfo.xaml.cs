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
	}
}
