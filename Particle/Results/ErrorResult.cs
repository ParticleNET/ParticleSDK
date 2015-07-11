using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Particle.Results
{
	internal class ErrorResult
	{
		[JsonProperty("error")]
		public String Error { get; set; }
		[JsonProperty("error_description")]
		public String ErrorDescription { get; set; }
	}
}
