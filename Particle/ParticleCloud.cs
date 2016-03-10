/*
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Particle.Results;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
#if WINDOWS_APP
using Windows.Web.Http;
using Windows.Web.Http.Filters;
#else
using System.Net.Http;
using System.Net;
#endif
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Particle
{
	/// <summary>
	/// Represents the connection to the particle cloud.
	/// </summary>
	public class ParticleCloud : ParticleBase, IDisposable
	{
		private HttpClient client;
		private Uri baseUri;
		private AuthenticationResults authResults;

		/// <summary>
		/// Gets the client.
		/// </summary>
		/// <value>
		/// The client.
		/// </value>
		protected HttpClient Client
		{
			get { return client; }
		}

		/// <summary>
		/// Gets the base URI.
		/// </summary>
		/// <value>
		/// The base URI.
		/// </value>
		protected Uri BaseUri
		{
			get { return baseUri; }
		}


		/// <summary>
		/// Set this to the UI threads SynchronizationContext
		/// Any operation that may update the UI gets sent through the SyncContext if its null the operation is executed on the current thread
		/// 
		/// i.e. for Universal App set this to ParticleCloud.SyncContext = System.Threading.SynchronizationContext.Current; in the App.xaml.cs inside the OnLaunch function
		/// </summary>
		public static SynchronizationContext SyncContext { get; set; }

		/// <summary>
		/// True if the user is authenticated into the cloud
		/// </summary>
		public bool IsAuthenticated
		{
			get
			{
				return authResults?.AccessToken?.Length > 0;
			}
		}

		/// <summary>
		/// Gets the users Access token or returns null if the user not authenticated.
		/// </summary>
		public String AccessToken
		{
			get
			{
				return authResults?.AccessToken;
			}
		}


		/// <summary>
		/// Gets the uri to the users Events. Usally https://api.particle.io/v1/devices/events
		/// </summary>
		/// <value>
		/// The Your event Uri.
		/// </value>
		public Uri YourEventUri
		{
			get
			{
				return GetFullUrl("devices/events");
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParticleCloud" /> class using the default url https://api.particle.io/v1/
		/// </summary>
		public ParticleCloud()
			: this(new Uri("https://api.particle.io/v1/"))
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParticleCloud" /> class.
		/// </summary>
		/// <param name="baseUri">The url to the particle sdk i.e. https://api.particle.io/v1/ </param>
		public ParticleCloud(Uri baseUri)
		{
			this.baseUri = baseUri;
#if WINDOWS_APP
			HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
			filter.CacheControl.ReadBehavior = HttpCacheReadBehavior.MostRecent;
			filter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.NoCache;
			filter.AllowUI = false;
			client = new HttpClient(filter);
#else
			client = new HttpClient();
			client.BaseAddress = baseUri;
#endif
		}


		/// <summary>
		/// Gets the full URL.
		/// </summary>
		/// <param name="method">The method like devices</param>
		/// <returns></returns>
		protected virtual Uri GetFullUrl(String method)
		{
			return new Uri(baseUri, method);
		}
		/// <summary>
		/// Makes the get request asynchronous to the cloud api
		/// </summary>
		/// <param name="method">The method to call i.e. devices</param>
		/// <returns>The results of the request.</returns>
		public virtual async Task<RequestResponse> MakeGetRequestAsync(String method)
		{
			if (String.IsNullOrWhiteSpace(method))
			{
				throw new ArgumentNullException(nameof(method));
			}

			if (authResults == null)
			{
				throw new ParticleAuthenticationExeption(String.Format(RH.C.GetString("YouMusthAuthenticateBeforeCalling"), method));
			}


			client.DefaultRequestHeaders.Clear();
#if WINDOWS_APP
			client.DefaultRequestHeaders.Authorization = new Windows.Web.Http.Headers.HttpCredentialsHeaderValue("Bearer", AccessToken);
			client.DefaultRequestHeaders.CacheControl.Add(new Windows.Web.Http.Headers.HttpNameValueHeaderValue("NO-CACHE"));
			var response = await client.GetAsync(GetFullUrl(method));
#else
			client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResults.AccessToken);
			client.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue()
			{
				NoCache = true
			};
			HttpResponseMessage response = await client.GetAsync(method);
#endif
			RequestResponse rr = new RequestResponse();
			var str = await response.Content.ReadAsStringAsync();
			rr.StatusCode = response.StatusCode;
			rr.Response = await Task.Run(() => JToken.Parse(str));
			return rr;
		}

		/// <summary>
		/// Calls <seealso cref="MakeGetRequestAsync(string)"/> and if it returns a status code of Unauthorized try s to refresh the token and makes the request again
		/// </summary>
		/// <param name="method">The method to call</param>
		/// <returns>The results from the request</returns>
		public async Task<RequestResponse> MakeGetRequestWithAuthTestAsync(String method)
		{
			var result = await MakeGetRequestAsync(method);
			if (result.StatusCode == HttpStatusCode.Unauthorized)
			{
				await RefreshTokenAsync();
				result = await MakeGetRequestAsync(method);
			}

			return result;
		}

		/// <summary>
		/// Makes the post request without authentication asynchronous to the particle cloud.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <param name="arguments">The arguments.</param>
		/// <returns>The Results of the request</returns>
		public virtual async Task<RequestResponse> MakePostRequestWithoutAuthAsync(String method, params KeyValuePair<String, String>[] arguments)
		{
			if (String.IsNullOrWhiteSpace(method))
			{
				throw new ArgumentNullException(nameof(method));
			}

			client.DefaultRequestHeaders.Clear();

			HttpResponseMessage response;
#if WINDOWS_APP
			if(arguments != null)
			{
				response = await client.PostAsync(GetFullUrl(method), new HttpFormUrlEncodedContent(arguments));
			}
			else
			{
				response = await client.PostAsync(GetFullUrl(method), null);
			}
#else
			if (arguments != null)
			{
				response = await client.PostAsync(method, new FormUrlEncodedContent(arguments));
			}
			else
			{
				response = await client.PostAsync(method, null);
			}
#endif
			var str = await response.Content.ReadAsStringAsync();
			RequestResponse rr = new RequestResponse();
			rr.StatusCode = response.StatusCode;
			rr.Response = await Task.Run(() => JToken.Parse(str));

			return rr;
		}

		/// <summary>
		/// Makes the post request asynchronous to the particle cloud
		/// </summary>
		/// <param name="method">The method to call</param>
		/// <param name="arguments">The arguments to pass during the call</param>
		/// <returns>The results of the request</returns>
		public virtual async Task<RequestResponse> MakePostRequestAsync(String method, params KeyValuePair<String, String>[] arguments)
		{
			if (String.IsNullOrWhiteSpace(method))
			{
				throw new ArgumentNullException(nameof(method));
			}

			if (authResults == null)
			{
				throw new ParticleAuthenticationExeption(String.Format(RH.C.GetString("YouMusthAuthenticateBeforeCalling"), method));
			}


			client.DefaultRequestHeaders.Clear();
#if WINDOWS_APP
			client.DefaultRequestHeaders.Authorization = new Windows.Web.Http.Headers.HttpCredentialsHeaderValue("Bearer", AccessToken);
#else
			client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResults.AccessToken);
#endif

			HttpResponseMessage response;
#if WINDOWS_APP
			if (arguments != null)
			{
				response = await client.PostAsync(GetFullUrl(method), new HttpFormUrlEncodedContent(arguments));
			}
			else
			{
				response = await client.PostAsync(GetFullUrl(method), null);
			}
#else
			if (arguments != null)
			{
				response = await client.PostAsync(method, new FormUrlEncodedContent(arguments));
			}
			else
			{
				response = await client.PostAsync(method, null);
			}
#endif
			var str = await response.Content.ReadAsStringAsync();
			RequestResponse rr = new RequestResponse();
			rr.StatusCode = response.StatusCode;
			rr.Response = await Task.Run(() => JToken.Parse(str));

			return rr;
		}

		/// <summary>
		/// Calls <seealso cref="MakePostRequestAsync(string, KeyValuePair{string, string}[])"/> and if it returns a status code of Unauthorized try s to refresh the token and makes the request again
		/// </summary>
		/// <param name="method">The method to call</param>
		/// <param name="arguments">The arguments to pass during the call</param>
		/// <returns>The results from the request</returns>
		public virtual async Task<RequestResponse> MakePostRequestWithAuthTestAsync(String method, params KeyValuePair<String, String>[] arguments)
		{
			var response = await MakePostRequestAsync(method, arguments);
			if (response.StatusCode == HttpStatusCode.Unauthorized)
			{
				await RefreshTokenAsync();
				response = await MakePostRequestAsync(method, arguments);
			}

			return response;
		}

		/// <summary>
		/// Makes the put request asynchronous to the particle cloud
		/// </summary>
		/// <param name="method">The method to call</param>
		/// <param name="arguments">The arguments to pass during the call</param>
		/// <returns>The results of the request</returns>
		public virtual async Task<RequestResponse> MakePutRequestAsync(String method, params KeyValuePair<String, String>[] arguments)
		{
			if (String.IsNullOrWhiteSpace(method))
			{
				throw new ArgumentNullException(nameof(method));
			}

			if (authResults == null)
			{
				throw new ParticleAuthenticationExeption(String.Format(RH.C.GetString("YouMusthAuthenticateBeforeCalling"), method));
			}

			client.DefaultRequestHeaders.Clear();
#if WINDOWS_APP
			client.DefaultRequestHeaders.Authorization = new Windows.Web.Http.Headers.HttpCredentialsHeaderValue("Bearer", AccessToken);
#else
			client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResults.AccessToken);
#endif

			HttpResponseMessage response;
#if WINDOWS_APP
			if(arguments != null)
			{
				response = await client.PutAsync(GetFullUrl(method), new HttpFormUrlEncodedContent(arguments));
			}
			else
			{
				response = await client.PutAsync(GetFullUrl(method), null);
			}
#else
			if (arguments != null)
			{
				response = await client.PutAsync(method, new FormUrlEncodedContent(arguments));
			}
			else
			{
				response = await client.PutAsync(method, null);
			}
#endif
			var str = await response.Content.ReadAsStringAsync();
			RequestResponse rr = new RequestResponse();
			rr.StatusCode = response.StatusCode;
			rr.Response = await Task.Run(() => JToken.Parse(str));

			return rr;
		}

		/// <summary>
		/// Calls <seealso cref="MakePutRequestAsync(string, KeyValuePair{string, string}[])"/> and if it returns a status code of Unauthorized try s to refresh the token and makes the request again
		/// </summary>
		/// <param name="method">The method to call</param>
		/// <param name="arguments">The arguments to pass during the call</param>
		/// <returns>The results from the request</returns>
		public virtual async Task<RequestResponse> MakePutRequestWithAuthTestAsync(String method, params KeyValuePair<String, String>[] arguments)
		{
			var response = await MakePutRequestAsync(method, arguments);
			if (response.StatusCode == HttpStatusCode.Unauthorized)
			{
				await RefreshTokenAsync();
				response = await MakePutRequestAsync(method, arguments);
			}

			return response;
		}

		/// <summary>
		/// Makes the delete request asynchronous to the particle cloud
		/// </summary>
		/// <param name="method">The method.</param>
		/// <returns></returns>
		public virtual async Task<RequestResponse> MakeDeleteRequestAsync(String method)
		{
			if (String.IsNullOrWhiteSpace(method))
			{
				throw new ArgumentNullException(nameof(method));
			}

			if (authResults == null)
			{
				throw new ParticleAuthenticationExeption(String.Format(RH.C.GetString("YouMusthAuthenticateBeforeCalling"), method));
			}

			client.DefaultRequestHeaders.Clear();
#if WINDOWS_APP
			client.DefaultRequestHeaders.Authorization = new Windows.Web.Http.Headers.HttpCredentialsHeaderValue("Bearer", AccessToken);
#else
			client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResults.AccessToken);
#endif

			HttpResponseMessage response;
#if WINDOWS_APP
			response = await client.DeleteAsync(GetFullUrl(method));
#else
			response = await client.DeleteAsync(method);
#endif
			var str = await response.Content.ReadAsStringAsync();
			RequestResponse rr = new RequestResponse();
			rr.StatusCode = response.StatusCode;
			rr.Response = await Task.Run(() => JToken.Parse(str));

			return rr;
		}

		/// <summary>
		/// Calls <seealso cref="MakeDeleteRequestAsync(string)"/> and if it returns a status code of Unauthorized try s to refresh the token and makes the request again
		/// </summary>
		/// <param name="method">The method to call</param>
		/// <returns>The results from the request</returns>
		public virtual async Task<RequestResponse> MakeDeleteRequestWithAuthTestAsync(String method)
		{
			var response = await MakeDeleteRequestAsync(method);
			if (response.StatusCode == HttpStatusCode.Unauthorized)
			{
				await RefreshTokenAsync();
				response = await MakeDeleteRequestAsync(method);
			}

			return response;
		}

		/// <summary>
		/// Refreshes the access token asynchronous.
		/// </summary>
		/// <returns>The results of the request to refresh the token</returns>
		public Task<Result> RefreshTokenAsync()
		{
			if (authResults == null)
			{
				throw new ParticleAuthenticationExeption(RH.C.GetString("YouMustLoginBeforeRefreshingToken"));
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
		/// <param name="username">The Particle account username</param>
		/// <param name="password">The Particle account password</param>
		/// <param name="expiresIn">How many seconds the token will be valid for. 0 means forever. Short lived tokens are better for security.</param>
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
#if WINDOWS_APP
			client.DefaultRequestHeaders.Authorization = new Windows.Web.Http.Headers.HttpCredentialsHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes("particle:particle")));
#else
			client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes("particle:particle")));
#endif

			var data = new Dictionary<String, String>();
			data["grant_type"] = "password";
			data["username"] = username;
			data["password"] = password;
			data["expires_in"] = expiresIn.ToString();
			// BaseAddress did not seam to work on Linux so changed to use UriBuilder
			UriBuilder b = new UriBuilder(baseUri);
			b.Path = "/oauth/token";
			try
			{
				var postResults = await client.PostAsync(b.Uri,
#if WINDOWS_APP
					new HttpFormUrlEncodedContent(data)
#else
					new FormUrlEncodedContent(data)
#endif
					);
				if (postResults.StatusCode ==
#if WINDOWS_APP
					HttpStatusCode.Ok
#else
					HttpStatusCode.OK
#endif
					)
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
							FirePropertyChanged(nameof(IsAuthenticated));
							return new Result
							{
								Success = true
							};
						}
					}
				}
				else if (postResults.StatusCode == HttpStatusCode.BadRequest)
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
			catch(Exception re)
			{
				return new Result
				{
					Success = false,
					Error = re.Message,
					Exception = re
				};
			}
		}

		/// <summary>
		/// Sign up with new account credentials to Particle cloud
		/// </summary>
		/// <param name="username">Required user name, must be a valid email address</param>
		/// <param name="password">Required password</param>
		/// <returns></returns>
		public async Task<Result> SignupWithUserAsync(String username, String password)
		{
			if (String.IsNullOrWhiteSpace(username))
			{
				throw new ArgumentNullException(nameof(username));
			}
			if (String.IsNullOrWhiteSpace(password))
			{
				throw new ArgumentNullException(nameof(password));
			}

			try
			{
				var result = await MakePostRequestWithoutAuthAsync("users", new KeyValuePair<string, string>("username", username), new KeyValuePair<string, string>("password", password));

				return result.AsResult();
			}
			catch (Exception re)
			{
				return new Result
				{
					Success = false,
					Error = re.Message,
					Exception = re
				};
			}
		}

		/// <summary>
		/// Requests the password be reset.
		/// </summary>
		/// <param name="email">The email.</param>
		/// <returns></returns>
		public async Task<Result> RequestPasswordResetAsync(String email)
		{
			if (String.IsNullOrWhiteSpace(email))
			{
				throw new ArgumentNullException(nameof(email));
			}

			try
			{
				var result = await MakePostRequestAsync("user/password-reset", new KeyValuePair<string, string>("username", email));

				return result.AsResult();
			}
			catch (Exception re)
			{
				return new Result
				{
					Success = false,
					Error = re.Message,
					Exception = re
				};
			}
		}

		/// <summary>
		/// Logs the user out locally
		/// </summary>
		public void Logout()
		{
			authResults = null;
			FirePropertyChanged(nameof(IsAuthenticated));
		}

		/// <summary>
		/// Get the list of devices the user has access to
		/// </summary>
		/// <returns></returns>
		public async Task<Result<List<ParticleDevice>>> GetDevicesAsync()
		{
			try
			{
				var response = await MakeGetRequestWithAuthTestAsync("devices");

				if (response.StatusCode ==
#if WINDOWS_APP
					HttpStatusCode.Ok
#else
					HttpStatusCode.OK
#endif
					)
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
			catch (Exception re)
			{
				return new Result<List<ParticleDevice>>(false, new List<ParticleDevice>())
				{
					Error = re.Message,
					Exception = re
				};
			}
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources mainly the underlining HttpClient
		/// </summary>
		public void Dispose()
		{
			client.Dispose();
		}

		// <summary>
		// Gets the device with the id equal to <paramref name="deviceId"/>
		// or returns null if its not found.
		// </summary>
		// <param name="deviceId">The id of the device to get</param>
		// <returns>The device</returns>
		//public async Task<ParticleDevice> GetDeviceAsync(String deviceId)
		//{

		//}

		// <summary>
		// Not available Yet
		// </summary>
		// <param name="eventName"></param>
		// <param name="data"></param>
		// <returns></returns>
		//public async Task PublishEvent(String eventName, System.IO.Stream data)
		//{

		//}

		/// <summary>
		/// Claims the specified device for the logged in user
		/// </summary>
		/// <param name="deviceId">The id of the new device</param>
		/// <returns></returns>
		public async Task<Result> ClaimDeviceAsync(String deviceId)
		{
			if (String.IsNullOrWhiteSpace(deviceId))
			{
				throw new ArgumentNullException(nameof(deviceId));
			}

			try
			{
				var result = await MakePostRequestWithAuthTestAsync("devices", new KeyValuePair<string, string>("id", deviceId));
				var userResult = result.AsResult();
				return userResult;
			}
			catch (Exception re)
			{
				return new Result
				{
					Success = false,
					Error = re.Message,
					Exception = re
				};
			}
		}

		// <summary>
		// Get a short-lived claiming token for transmitting to soon-to-be-claimed device in soft AP setup process
		// </summary>
		//public async Task<ClaimCodeResult> GenerateClaimCodeAsync()
		//{

		//}


	}
}
