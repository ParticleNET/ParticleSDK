using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Win8;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Universal
{
	public sealed partial class AuthenticationDialog : ContentDialog
	{
		public AuthenticationDialog()
		{
			this.InitializeComponent();
			DataContext = AppSettings.Current;
		}

		private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			ErrorContainer.Visibility = Visibility.Collapsed;
			var deferral = args.GetDeferral();
			LoggingInContainer.Visibility = Visibility.Visible;
			var result = await App.Cloud.LoginWithUserAsync(AppSettings.Current.Username, AppSettings.Current.Password);
			if (!result.Success)
			{
				args.Cancel = true;
				ErrorOutput.Text = result.Error;
				ErrorDescriptionOutput.Text = result.ErrorDescription;
				ErrorContainer.Visibility = Visibility.Visible;
			}
			LoggingInContainer.Visibility = Visibility.Collapsed;
			deferral.Complete();
		}

		private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
		}
	}
}
