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
		/// Converts the JSon response to a Result
		/// </summary>
		/// <param name="success">if set to <c>true</c> [success].</param>
		/// <returns></returns>
		public Result AsResult(bool success = false)
		{
			var result = Response.ToObject<Result>();
			result.Success = success;
			return result;
		}

		/// <summary>
		/// Converts the JSon response to a Result
		/// </summary>
		/// <typeparam name="T">Type of the results</typeparam>
		/// <param name="success">if set to <c>true</c> [success].</param>
		/// <returns></returns>
		public Result<T> AsResult<T>(bool success = false)
		{
			var result = Response.ToObject<Result<T>>();
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
