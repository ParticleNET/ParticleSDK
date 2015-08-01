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
	public partial class DeviceInformationPage : Page
	{
		public DeviceInformationPage()
		{
			this.InitializeComponent();
		}

		protected async override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			var pd = e.Parameter as ParticleDevice;
			if(pd == null)
			{
				Frame.GoBack();
			}
			else
			{
				try
				{
					await pd.RefreshAsync();
					DataContext = pd;
				}
				catch(ParticleException pe)
				{
					var dialog = new MessageDialog(pe.ErrorDescription);
					await dialog.ShowAsync();
				}
			}
		}
	}
}
