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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Particle
{
	/// <summary>
	/// Represents a variable for a device
	/// </summary>
	public class Variable : ParticleBase
	{
		private ParticleDevice device;

		/// <summary>
		/// Initializes a new instance of the <see cref="Variable"/> class.
		/// </summary>
		/// <param name="device">The device.</param>
		public Variable(ParticleDevice device)
		{
			this.device = device;
		}

		private String name;
		/// <summary>
		/// Gets the name of the variable
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public String Name
		{
			get { return name; }
			internal set { SetProperty(ref name, value); }
		}

		private VariableType type;
		/// <summary>
		/// Gets the type of the variable
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		public VariableType Type
		{
			get { return type; }
			internal set { SetProperty(ref type, value); }
		}

		private String value;
		/// <summary>
		/// Gets the value of the variable
		/// </summary>
		/// <value>
		/// The value.
		/// </value>
		public String Value
		{
			get { return value; }
			internal set { SetProperty(ref this.value, value); }
		}

		/// <summary>
		/// Refreshes the value asynchronous.
		/// </summary>
		/// <returns></returns>
		public async Task<Result> RefreshValueAsync()
		{
			return (Result)await device.GetVariableValueAsync(this);
		}
	}
}
