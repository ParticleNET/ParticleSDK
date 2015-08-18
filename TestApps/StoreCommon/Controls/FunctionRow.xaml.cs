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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Common.Controls
{
	public sealed partial class FunctionRow : UserControl
	{
		public ParticleDevice Device
		{
			get { return (ParticleDevice)GetValue(DeviceProperty); }
			set { SetValue(DeviceProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Device.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty DeviceProperty =
			DependencyProperty.Register("Device", typeof(ParticleDevice), typeof(FunctionRow), new PropertyMetadata(null));



		public String FunctionName
		{
			get { return (String)GetValue(FunctionNameProperty); }
			set
			{
				SetValue(FunctionNameProperty, value);
				NameOutput.Text = value;
			}
		}

		// Using a DependencyProperty as the backing store for FunctionName.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty FunctionNameProperty =
			DependencyProperty.Register("FunctionName", typeof(String), typeof(FunctionRow), new PropertyMetadata(null, new PropertyChangedCallback(FunctionNameChange)));

		public static void FunctionNameChange(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			(obj as FunctionRow).OnFunctionNameChanged(args.NewValue as String);
		}

		private void OnFunctionNameChanged(String newValue)
		{
			NameOutput.Text = newValue ?? "";
		}


		public FunctionRow()
		{
			this.InitializeComponent();
		}



		private async void CallAction_Tapped(object sender, TappedRoutedEventArgs e)
		{
			Busy.Visibility = Visibility.Visible;
			Busy.IsActive = true;
			var results = await Device.CallFunctionAsync(FunctionName, ArgumentInput.Text);
			if (!results.Success)
			{
				var dialog = new MessageDialog(results.ErrorDescription, results.Error);
				await dialog.ShowAsync();
			}
			else
			{
				ReturnOutput.Text = results.Data.ToString();
			}
			Busy.IsActive = false;
			Busy.Visibility = Visibility.Collapsed;
		}
	}
}
