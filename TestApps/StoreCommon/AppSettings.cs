using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Common
{
	public class AppSettings : INotifyPropertyChanged
	{
		private ApplicationDataContainer settings;

		public static AppSettings Current { get; } = new AppSettings();

		private AppSettings()
		{
			settings = ApplicationData.Current.LocalSettings;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void firePropertyChanged(String property)
		{
			if (PropertyChanged != null)
			{
				lock (PropertyChanged)
				{
					PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(property));
				}
			}
		}

		private void setBoolean(bool value, [CallerMemberName]String key = null)
		{
			settings.Values[key] = value;
			firePropertyChanged(key);
		}

		private int getInt([CallerMemberName]String key = null, int def = 0)
		{
			int? b = settings.Values[key] as int?;
			if (b.HasValue)
			{
				return b.Value;
			}

			settings.Values[key] = def;

			return def;
		}

		private void setInt(int value, [CallerMemberName]String key = null)
		{
			settings.Values[key] = value;
			firePropertyChanged(key);
		}

		private long getLong([CallerMemberName]String key = null, long def = 0)
		{
			long? b = settings.Values[key] as long?;
			if (b.HasValue)
			{
				return b.Value;
			}

			settings.Values[key] = def;

			return def;
		}

		private void setLong(long value, [CallerMemberName]String key = null)
		{
			settings.Values[key] = value;
			firePropertyChanged(key);
		}

		private String getString([CallerMemberName]String key = null, String def = null)
		{
			String s = settings.Values[key] as String;
			if (s != null)
			{
				return s;
			}

			return def;
		}

		private void setString(String value, [CallerMemberName]String key = null)
		{
			settings.Values[key] = value;
		}

		public String Username
		{
			get
			{
				return getString();
			}
			set
			{
				setString(value);
			}
		}

		public String Password
		{
			get
			{
				return getString();
			}
			set
			{
				setString(value);
			}
		}
	}
}
