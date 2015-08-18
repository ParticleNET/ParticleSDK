/*
Copyright 2015 Sannel Software, L.L.C.

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

		public string Username { get; set; }

		public string Password { get; set; }
	}
}
