﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Common.Controls
{
	public class AlternatingRowComboBox : ComboBox
	{
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			base.PrepareContainerForItemOverride(element, item);
			var listViewItem = element as ListViewItem;

			if (listViewItem != null)
			{
				var index = IndexFromContainer(element);

				if ((index) == 1)
				{
					listViewItem.Background = new SolidColorBrush(Colors.White);
				}
				else
				{
					listViewItem.Background = new SolidColorBrush(Colors.Gray);
				}
			}
		}
	}
}
