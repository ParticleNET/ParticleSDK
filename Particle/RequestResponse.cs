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

		public Result AsResult()
		{
			return Response.ToObject<Result>();
		}

		public ParticleException AsParticleException(String message)
		{
			var result = AsResult();
			return new ParticleException(message, StatusCode, result.Error, result.ErrorDescription);
		}
	}
}
