using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Particle.Results
{
	internal class AuthenticationResults
	{
		[JsonProperty("token_type")]
		public String TokenType { get; set; }
		[JsonProperty("access_token")]
		public String AccessToken { get; set; }
		[JsonProperty("expires_in")]
		public int ExpiresIn { get; set; }
		[JsonProperty("refresh_token")]
		public String RefreshToken { get; set; }
	}
}
