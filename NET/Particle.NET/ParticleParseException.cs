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
	/// Fires when there's an exception parsing json from the particle cloud
	/// </summary>
	/// <seealso cref="System.Exception" />
	public class ParticleParseException : Exception
	{
		/// <summary>
		/// Gets or sets the Source json that caused the exception to be thrown
		/// </summary>
		/// <value>
		/// The Source json.
		/// </value>
		public String SourceJson { get; set; }

		/// <summary>
		/// Gets or sets the name of the property that was last parsed when the exception was thrown
		/// </summary>
		/// <value>
		/// The name of the property.
		/// </value>
		public String PropertyName { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ParticleParseException"/> class.
		/// </summary>
		public ParticleParseException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParticleParseException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public ParticleParseException(string message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParticleParseException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
		public ParticleParseException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
