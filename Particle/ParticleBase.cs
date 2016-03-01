/*
Copyright 2016 ParticleNET

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

	http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
 */
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Particle
{
	/// <summary>
	/// The base class for the ParticleCloud and ParticleDevice
	/// </summary>
	public abstract class ParticleBase : INotifyPropertyChanged
	{
		/// <summary>
		/// Occurs when a [property changed].
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
