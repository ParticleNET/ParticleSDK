using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Particle
{
	public abstract class ParticleBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void FirePropertyChanged(String name)
		{
			if (PropertyChanged != null)
			{
				lock (PropertyChanged)
				{
					ParticleCloud.SyncContext.InvokeIfRequired(() =>
					{
						PropertyChanged(this, new PropertyChangedEventArgs(name));
					});
				}
			}
		}

		protected bool SetProperty<T>(ref T store, T value, [CallerMemberName] String name = null)
		{
			if(Object.Equals(value, store))
			{
				return false;
			}

			store = value;
			FirePropertyChanged(name);
			return true;
		}
	}
}
