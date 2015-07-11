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
using Particle.Results;
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

		public ParticleCloud()
			: this(new Uri("https://api.particle.io/v1/"))
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
		/// <summary>
		/// Sign up with new account credentials to Spark cloud
		/// </summary>
		/// <param name="username">Required user name, must be a valid email address</param>
		/// <param name="password">Required password</param>
		/// <returns></returns>
		//public async Task<SignupResults> SignupWithUserAsync(String username, String password)
		//{

		//}

		/// <summary>
		/// Logs the user out locally
		/// </summary>
		public void Logout()
		{
			authResults = null;
		}

		/// <summary>
		/// Get the list of devices the user has claimed
		/// </summary>
		/// <returns></returns>
		//public async Task<List<ParticleDevice>> GetDevicesAsync()
		//{

		//}

		/// <summary>
		/// Gets the device with the id equal to <paramref name="deviceId"/>
		/// or returns null if its not found.
		/// </summary>
		/// <param name="deviceId">The id of the device to get</param>
		/// <returns>The device</returns>
		//public async Task<ParticleDevice> GetDeviceAsync(String deviceId)
		//{

		//}

		/// <summary>
		/// Not available Yet
		/// </summary>
		/// <param name="eventName"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		//public async Task PublishEvent(String eventName, System.IO.Stream data)
		//{

		//}

		/// <summary>
		/// Claims the specified device for the logged in user
		/// </summary>
		/// <param name="deviceId">The id of the new device</param>
		/// <returns></returns>
		//public async Task<ClaimResult> ClaimDeviceAsync(String deviceId)
		//{

		//}

		/// <summary>
		/// Get a short-lived claiming token for transmitting to soon-to-be-claimed device in soft AP setup process
		/// </summary>
		/// <returns></returns>
		//public async Task<ClaimCodeResult> GenerateClaimCodeAsync()
		//{

		//}



		public void Dispose()
		{
			client.Dispose();
		}
	}
}
