using Newtonsoft.Json;
using Particle.Results;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Particle
{
	public class ParticleCloud : IDisposable
	{
		private HttpClient client;
		private Uri baseUri;
		private AuthenticationResults authResults;

		public ParticleCloud() : this(new Uri("https://api.particle.io/"))
		{

		}

		public ParticleCloud(Uri baseUri)
		{
			this.baseUri = baseUri;
			client = new HttpClient();
			client.BaseAddress = baseUri;
		}

		public async Task<LoginResult> LoginWithUserAsync(String username, String password)
		{
			if (String.IsNullOrWhiteSpace(username))
			{
				throw new ArgumentNullException("username");
			}
			if (String.IsNullOrWhiteSpace(password))
			{
				throw new ArgumentNullException("password");
			}
			
			client.DefaultRequestHeaders.Clear();
			client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes("particle:particle")));
			var data = new Dictionary<String, String>();
			data["grant_type"] = "password";
			data["username"] = username;
			data["password"] = password;
			var postResults = await client.PostAsync("/oauth/token", new FormUrlEncodedContent(data));
			if (postResults.StatusCode == System.Net.HttpStatusCode.OK)
			{
				var results = await postResults.Content.ReadAsStringAsync();
				var ret = await Task.Run(() => JsonConvert.DeserializeObject<AuthenticationResults>(results));
				if (ret != null)
				{
					if (!String.IsNullOrWhiteSpace(ret.AccessToken))
					{
						authResults = ret;
						return new LoginResult
						{
							IsAuthenticated = true
						};
					}
				}
			}
			else if (postResults.StatusCode == System.Net.HttpStatusCode.BadRequest)
			{
				var results = await postResults.Content.ReadAsStringAsync();
				var ret = await Task.Run(() => JsonConvert.DeserializeObject<ErrorResult>(results));
				if (ret != null)
				{
					return new LoginResult
					{
						IsAuthenticated = false,
						Error = ret.ErrorDescription
					};
				}
			}

			return new LoginResult
			{
				IsAuthenticated = false,
				Error = postResults.StatusCode.ToString()
			};
		}

		public void Dispose()
		{
			
		}
	}
}
