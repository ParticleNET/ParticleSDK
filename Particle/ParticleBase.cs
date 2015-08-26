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
	/// <summary>
	/// The base class for the ParticleCloud and ParticleDevice
	/// </summary>
	public abstract class ParticleBase : INotifyPropertyChanged
	{
		/// <summary>
		/// Occurs when [property changed].
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Fires the property changed.
		/// </summary>
		/// <param name="name">The name.</param>
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

		/// <summary>
		/// Sets the property and fires <seealso cref="FirePropertyChanged(string)"/>
		/// </summary>
		/// <typeparam name="T">The type of the property</typeparam>
		/// <param name="store">The store.</param>
		/// <param name="value">The value.</param>
		/// <param name="name">The name.</param>
		/// <returns></returns>
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
