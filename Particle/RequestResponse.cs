﻿/*
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
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Particle
{
	/// <summary>
	/// The Response from the Particle Cloud API
	/// </summary>
	public class RequestResponse
	{
		/// <summary>
		/// The status code from the request
		/// </summary>
		/// <value>
		/// The status code.
		/// </value>
		public HttpStatusCode StatusCode { get; set; }

		/// <summary>
		/// The JSon Response from the cloud
		/// </summary>
		/// <value>
		/// The response.
		/// </value>
		public JToken Response { get; set; }

		/// <summary>
		/// Gets or sets the exception.
		/// </summary>
		/// <value>
		/// The exception.
		/// </value>
		public Exception Exception { get; set; }

		/// <summary>
		/// Converts the JSon response to a Result
		/// </summary>
		/// <returns></returns>
		public Result AsResult()
		{
			var result = Response.ToObject<Result>();
			return result;
		}

		/// <summary>
		/// Converts the JSon response to a Result
		/// </summary>
		/// <param name="success">if set to <c>true</c> [success].</param>
		/// <returns></returns>
		public Result AsResult(bool success)
		{
			var result = AsResult();
			result.Success = success;
			return result;
		}

		/// <summary>
		/// Converts the JSon response to a Result
		/// </summary>
		/// <typeparam name="T">Type of the results</typeparam>
		/// <returns></returns>
		public Result<T> AsResult<T>()
		{
			var result = Response.ToObject<Result<T>>();
			return result;
		}

		/// <summary>
		/// Converts the JSon response to a Result
		/// </summary>
		/// <typeparam name="T">Type of the results</typeparam>
		/// <param name="success">if set to <c>true</c> [success].</param>
		/// <returns></returns>
		public Result<T> AsResult<T>(bool success)
		{
			var result = AsResult<T>();
			result.Success = success;
			return result;
		}

		/// <summary>
		/// Converts the JSon response to a ParticleException
		/// </summary>
		/// <param name="message">The message.</param>
		/// <returns></returns>
		public ParticleException AsParticleException(String message)
		{
			var result = AsResult();
			return new ParticleException(message, StatusCode, result.Error, result.ErrorDescription);
		}
	}
}
