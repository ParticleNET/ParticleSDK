using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
		}

		private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			var deferral = args.GetDeferral();
			var results = await AuthControl.AuthenticateAsync(App.Cloud);
			if(!results)
			{
				args.Cancel = true;
			}
			deferral.Complete();
		}
	}
}
