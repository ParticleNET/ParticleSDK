using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Particle
{
	public class RequestResponse
	{
		public HttpStatusCode StatusCode { get; set; }

		public JToken Response { get; set; }

		public Result AsResult(bool success = false)
		{
			var result = Response.ToObject<Result>();
			result.Success = success;
			return result;
		}

		public Result<T> AsResult<T>(bool success = false)
		{
			var result = Response.ToObject<Result<T>>();
			result.Success = success;
			return result;
		}

		public ParticleException AsParticleException(String message)
		{
			var result = AsResult();
			return new ParticleException(message, StatusCode, result.Error, result.ErrorDescription);
		}
	}
}
