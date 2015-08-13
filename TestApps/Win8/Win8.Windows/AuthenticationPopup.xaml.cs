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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Win8
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class AuthenticationPopup : UserControl
	{
		public event EventHandler Authenticated;


		public AuthenticationPopup()
		{
			this.InitializeComponent();
		}

		private void fireAuthenticated()
		{
			if(Authenticated != null)
			{
				lock (Authenticated)
				{
					Authenticated(this, EventArgs.Empty);
				}
			}
		}

		public void Show()
		{
			Container.IsOpen = true;
		}

		private async void LoginAction_Tapped(object sender, TappedRoutedEventArgs e)
		{
			var result = await AuthControl.AuthenticateAsync(App.Cloud);
			if(result)
			{
				fireAuthenticated();
				Container.IsOpen = false;
			}
		}
	}
}
