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
using Newtonsoft.Json.Linq;
using Particle.Results;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Particle
{
	public class ParticleCloud : ParticleBase, IDisposable
	{
		private HttpClient client;
		private Uri baseUri;
		private AuthenticationResults authResults;

		public static SynchronizationContext SyncContext { get; set; }

		public bool IsAuthenticated
		{
			get
			{
				return authResults?.AccessToken?.Length > 0;
			}
		}

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

		public virtual async Task<RequestResponse> MakeGetRequestAsync(String method)
		{
			if (String.IsNullOrWhiteSpace(method))
			{
				throw new ArgumentNullException(nameof(method));
			}

			if (authResults == null)
			{
				throw new ParticleAuthenticationExeption(String.Format(Messages.YouMusthAuthenticateBeforeCalling, method));
			}


			client.DefaultRequestHeaders.Clear();
			client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResults.AccessToken);
			HttpResponseMessage response = await client.GetAsync(method);
			var str = await response.Content.ReadAsStringAsync();
			RequestResponse rr = new RequestResponse();
			rr.StatusCode = response.StatusCode;
			rr.Response = await Task.Run(() => JToken.Parse(str));

			return rr;
		}

		public async Task<RequestResponse> MakeGetRequestWithAuthTestAsync(String method)
		{
			var result = await MakeGetRequestAsync(method);
			if(result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
			{
				await RefreshTokenAsync();
				result = await MakeGetRequestAsync(method);
			}

			return result;
		}

		public virtual async Task<RequestResponse> MakePostRequestAsync(String method, params KeyValuePair<String, String>[] arguments)
		{
			if (String.IsNullOrWhiteSpace(method))
			{
				throw new ArgumentNullException(nameof(method));
			}
			
			if(authResults == null)
			{
				throw new ParticleAuthenticationExeption(String.Format(Messages.YouMusthAuthenticateBeforeCalling, method));
			}

			
			client.DefaultRequestHeaders.Clear();
			client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResults.AccessToken);

			HttpResponseMessage response;
			if (arguments != null)
			{
				response = await client.PostAsync(method, new FormUrlEncodedContent(arguments));
			}
			else
			{
				response = await client.PostAsync(method, null);
			}
			var str = await response.Content.ReadAsStringAsync();
			RequestResponse rr = new RequestResponse();
			rr.StatusCode = response.StatusCode;
			rr.Response = await Task.Run(() => JToken.Parse(str));

			return rr;
		}

		public virtual async Task<RequestResponse> MakePostRequestWithAuthTestAsync(String method, params KeyValuePair<String, String>[] arguments)
		{
			var response = await MakePostRequestAsync(method, arguments);
			if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
			{
				await RefreshTokenAsync();
				response = await MakePostRequestAsync(method, arguments);
			}

			return response;
		}

		public Task<Result> RefreshTokenAsync()
		{
			if (authResults == null)
			{
				throw new ParticleAuthenticationExeption(Messages.YouMustLoginBeforeRefreshingToken);
			}

			return LoginWithUserAsync(authResults.Username, authResults.Password, authResults.ExpiresIn);
			/* The ideal way below does not currently appear to be currently supported.
			client.DefaultRequestHeaders.Clear();
			client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes("particle:particle")));

			var data = new Dictionary<String, String>();
			data["refresh_token"] = authResults.RefreshToken;
			data["grant_type"] = "refresh_token";
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
						return new Result
						{
							Success = true
						};
					}
				}
			}
			else if (postResults.StatusCode == System.Net.HttpStatusCode.BadRequest)
			{
				var results = await postResults.Content.ReadAsStringAsync();
				var ret = await Task.Run(() => JsonConvert.DeserializeObject<Result>(results));
				if (ret != null)
				{
					ret.Success = false;
					return ret;
				}
			}

			return new Result
			{
				Success = false,
				Error = postResults.StatusCode.ToString()
			};*/
		}

		/// <summary>
		/// Logs into ParticleCloud with the given username and password
		/// </summary>
		/// <param name="username">Must be a valid email address</param>
		/// <param name="password">The password for the user</param>
		/// <returns>Result if Result.Success == true the user is logged in if its false the user is not logged in and ErrorMessage will contain the error from the server</returns>
		public async Task<Result> LoginWithUserAsync(String username, String password, int expiresIn = 3600)
		{
			if (String.IsNullOrWhiteSpace(username))
			{
				throw new ArgumentNullException(nameof(username));
			}
			if (String.IsNullOrWhiteSpace(password))
			{
				throw new ArgumentNullException(nameof(password));
			}

			client.DefaultRequestHeaders.Clear();
			client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes("particle:particle")));

			var data = new Dictionary<String, String>();
			data["grant_type"] = "password";
			data["username"] = username;
			data["password"] = password;
			data["expires_in"] = expiresIn.ToString();
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
						authResults.Username = username;
						authResults.Password = password;
						return new Result
						{
							Success = true
						};
					}
				}
			}
			else if (postResults.StatusCode == System.Net.HttpStatusCode.BadRequest)
			{
				var results = await postResults.Content.ReadAsStringAsync();
				var ret = await Task.Run(() => JsonConvert.DeserializeObject<Result>(results));
				if (ret != null)
				{
					ret.Success = false;
					return ret;
				}
			}

			return new Result
			{
				Success = false,
				Error = postResults.StatusCode.ToString()
			};
		}

		/// <summary>
		/// Sign up with new account credentials to Particle cloud
		/// </summary>
		/// <param name="username">Required user name, must be a valid email address</param>
		/// <param name="password">Required password</param>
		/// <returns></returns>
		public async Task<Result> SignupWithUserAsync(String username, String password)
		{
			// /v1/users/signUp
			// Post:
			// username
			// password
			return null;
		}

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
		public async Task<Result<List<ParticleDevice>>> GetDevicesAsync()
		{
			var response = await MakeGetRequestAsync("devices");
			if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
			{
				await RefreshTokenAsync();
				 response = await MakeGetRequestAsync("devices");
				 if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
				 {
					return response.AsResult<List<ParticleDevice>>();
				 }
			}

			if (response.StatusCode == System.Net.HttpStatusCode.OK)
			{
				List<ParticleDevice> items = new List<ParticleDevice>();
				await Task.Run(() =>
					{
						foreach (JObject obj in (JArray)response.Response)
						{
							items.Add(new ParticleDevice(this, obj));
						}
					});

				return new Result<List<ParticleDevice>>(true, items);
			}
			else
			{
				return response.AsResult<List<ParticleDevice>>();
			}
		}

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
