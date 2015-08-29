/*
Copyright 2015 ParticleNET

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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Particle
{
	/// <summary>
	/// The results from the request.
	/// </summary>
	public class Result
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Result"/> class.
		/// </summary>
		public Result()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Result"/> class.
		/// </summary>
		/// <param name="success">if set to <c>true</c> the request was successful</param>
		public Result(bool success)
		{
			Success = success;
		}

		/// <summary>
		/// Gets or sets the message.
		/// </summary>
		/// <value>
		/// The message.
		/// </value>
		[JsonProperty("message")]
		public String Message { get; set; }


		/// <summary>
		/// Redirect info in the json to the Massage property<seealso cref="Message"/> 
		/// </summary>
		/// <value>
		/// <seealso cref="Message" />
		/// </value>
		[JsonProperty("info")]
		public String Info
		{
			set
			{
				Message = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Result"/> is success.
		/// </summary>
		/// <value>
		///   <c>true</c> if success; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("ok")]
		public bool Success { get; set; }
		/// <summary>
		/// Gets or sets the error.
		/// </summary>
		/// <value>
		/// The error.
		/// </value>
		[JsonProperty("error")]
		public String Error
		{
			get
			{
				if(Errors.Count > 0)
				{
					return Errors[0];
				}

				return null;
			}
			set
			{
				if (Errors.Count <= 0)
				{
					Errors.Add(value);
				}
				else
				{
					Errors[0] = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the error description.
		/// </summary>
		/// <value>
		/// The error description.
		/// </value>
		[JsonProperty("error_description")]
		public String ErrorDescription
		{
			get
			{
				if(Errors.Count > 1)
				{
					return Errors[1];
				}

				return null;
			}
			set
			{
				if(Errors.Count <= 0)
				{
					Errors.Add("");
					Errors.Add(value);
				}
				else if(Errors.Count  == 1)
				{
					Errors.Add(value);
				}
				else
				{
					Errors[1] = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the errors.
		/// </summary>
		/// <value>
		/// The errors.
		/// </value>
		[JsonProperty("errors")]
		public List<String> Errors { get; private set; } = new List<string>();
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T">The Type representing the Data</typeparam>
	public class Result<T> : Result
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Result{T}"/> class.
		/// </summary>
		public Result()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Result{T}"/> class.
		/// </summary>
		/// <param name="success">if set to <c>true</c> the request was successful</param>
		public Result(bool success) : base(success) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Result{T}"/> class.
		/// </summary>
		/// <param name="success">if set to <c>true</c> [success].</param>
		/// <param name="data">The data.</param>
		public Result(bool success, T data) : base(success)
		{
			Data = data;
		}

		/// <summary>
		/// Gets or sets the data.
		/// </summary>
		/// <value>
		/// The data.
		/// </value>
		public T Data { get; set; }
	}
}
