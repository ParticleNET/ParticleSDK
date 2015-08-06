using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Universal
{
	public class ComboBoxItemTemplateSelector : DataTemplateSelector
	{
		public DataTemplate SelectedTemplate { get; set; }
		public DataTemplate DropDownTemplate { get; set; }

		protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
		{
			var comboBoxItem = container.GetVisualParent<ComboBoxItem>();
			if (comboBoxItem == null)
			{
				return SelectedTemplate;
			}
			return DropDownTemplate;
		}

	}

	public static class DependencyObjectExtensions
	{
		public static T GetVisualParent<T>(this DependencyObject child) where T : FrameworkElement
		{
			while ((child != null) && !(child is T))
			{
				child = VisualTreeHelper.GetParent(child);
			}
			return child as T;
		}
	}
}
