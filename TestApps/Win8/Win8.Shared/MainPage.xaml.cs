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

namespace Win8
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
			DataContext = AppSettings.Current;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
			
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
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
					MessageDialog dialog = new MessageDialog("Login Successful");
					await dialog.ShowAsync();
				}
			}
			catch(ParticleException ex)
			{
				var dialog = new MessageDialog(ex.ToString());
				await dialog.ShowAsync();
			}

			Authenticating.Visibility = Visibility.Collapsed;
        }
	}
}
