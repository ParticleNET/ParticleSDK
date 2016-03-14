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
#if NETFX_CORE
using Windows.Web.Http;
#else
using System.Net;
#endif
using Newtonsoft.Json;

namespace Particle
{
	/// <summary>
	/// 
	/// </summary>
	public class ParticleException : Exception
	{
		/// <summary>
		/// Gets or sets the status.
		/// </summary>
		/// <value>
		/// The status.
		/// </value>
		public HttpStatusCode Status{ get; set; }
		/// <summary>
		/// Gets or sets the error.
		/// </summary>
		/// <value>
		/// The error.
		/// </value>
		[JsonProperty("error")]
		public String Error { get; set; }
		/// <summary>
		/// Gets or sets the error description.
		/// </summary>
		/// <value>
		/// The error description.
		/// </value>
		[JsonProperty("error_description")]
		public String ErrorDescription { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ParticleException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public ParticleException(String message)
			: base(message)
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParticleException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="code">The code.</param>
		/// <param name="error">The error.</param>
		/// <param name="errorDescription">The error description.</param>
		public ParticleException(String message, HttpStatusCode code, String error, String errorDescription)
			: base(message)
		{
			this.Status = code;
			this.Error = error;
			this.ErrorDescription = errorDescription;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParticleException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
		public ParticleException(String message, Exception innerException)
			: base(message, innerException)
		{

		}
	}
}
