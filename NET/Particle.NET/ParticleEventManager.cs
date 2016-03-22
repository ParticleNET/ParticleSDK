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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
#if NETFX_CORE
using Windows.Web.Http;
using Windows.Web.Http.Filters;
#else
using System.Net.Http;
#endif
using System.Text;
using System.Threading.Tasks;

namespace Particle
{
	/// <summary>
	/// This class is used to manage the stream of events from the Particle Cloud and Turn them into native .net events
	/// </summary>
	public class ParticleEventManager
	{
		/// <summary>
		/// Used to determen if we should keep running.
		/// 
		/// If this is false we keep restarting and connecting to the event service
		/// If this is true stop listening to the event service
		/// </summary>
		protected bool _stop = true;
		private bool hasConnected = false;
		private StreamReader reader;
		private Uri streamUri;
		private String accessToken;
		/// <summary>
		/// Occurs when an event is encountered from the particle cloud api
		/// </summary>
		public event EventHandler<WebEventArgs> Events;

		/// <summary>
		/// Occurs when there has been an error from the connection
		/// </summary>
		public event Action<Exception> Error;

		/// <summary>
		/// Occurs when we are going to try and reconnect to the client
		/// </summary>
		public event Action Reconnecting;

		/// <summary>
		/// Occurs when we have reconnected to the client
		/// </summary>
		public event Action Reconnected;

		/// <summary>
		/// Occurs when we have connected for the first time.
		/// </summary>
		public event Action Connected;

		/// <summary>
		/// Occurs when we stop listening to events
		/// </summary>
		public event Action Closed;
#if !NETFX_CORE
		/// <summary>
		/// How long in Milliseconds to wait before a read timeout occurs
		/// If ReadTimeout is set to 0 Timeout is not set. On Windows Phone the stream does not support this timeout on the stream.
		/// Default: 36000
		/// </summary>
		public int ReadTimeout { get; set; } = 36000;
#endif
		/// <summary>
		/// How long to delay before reconnecting after being disconnected in Milliseconds
		/// Default: 1000
		/// </summary>
		public int ReconnectDelay { get; set; } = 1000;

		/// <summary>
		/// Gets a value indicating whether this instance is running.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is running; otherwise, <c>false</c>.
		/// </value>
		public bool IsRunning
		{
			get
			{
				return !_stop;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParticleEventManager"/> class. Without requiring the Access Token, or Uri mostly used for unit tests
		/// </summary>
		protected ParticleEventManager()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParticleEventManager"/> class.
		/// </summary>
		/// <param name="streamUri">The stream URI. i.e. https://api.particle.io/v1/devices/events </param>
		/// <param name="accessToken">The access token for authentication.</param>
		public ParticleEventManager(Uri streamUri, String accessToken)
		{
			this.streamUri = streamUri;
			this.accessToken = accessToken;
		}

		/// <summary>
		/// Fires the event.
		/// </summary>
		/// <param name="eventName">Name of the event.</param>
		/// <param name="data">The data.</param>
		protected async void fireEvent(String eventName, ParticleEventData[] data)
		{
			await Task.Run(() =>
			{
				Events?.Invoke(this, new WebEventArgs
				{
					Event = eventName,
					Data = data
				});
			});
			
		}

		/// <summary>
		/// Starts listening to the events coming in from the specified stream
		/// </summary>
		public void Start()
		{
			_stop = false;
			Task.Factory.StartNew(()=>StartAsync().ConfigureAwait(false), TaskCreationOptions.LongRunning);
			
		}

		/// <summary>
		/// Start the connection loop to connect to the event api.
		/// </summary>
		/// <returns></returns>
		protected virtual async Task StartAsync()
		{
			while (!_stop) // If we have not been told to stop reconnect to the stream if an exception has occurred
			{
				try
				{
					await ConnectToClient();
				}
				catch (Exception ex)
				{
					Error?.Invoke(ex);
					await Task.Delay(ReconnectDelay); // try to delay restart so we dont overwhelm anything
				}
			}

			Closed?.Invoke();
		}

		/// <summary>
		/// Connects to client.
		/// </summary>
		/// <returns>A Task that can be awaited</returns>
		protected virtual async Task ConnectToClient()
		{
#if NETFX_CORE
			HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
			filter.CacheControl.ReadBehavior = HttpCacheReadBehavior.MostRecent;
			filter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.NoCache;
			filter.AllowUI = false;
			using (HttpClient client = new HttpClient())
			{
				if (hasConnected)
				{
					Reconnecting?.Invoke();
				}
				client.DefaultRequestHeaders.Authorization = new Windows.Web.Http.Headers.HttpCredentialsHeaderValue("Bearer", accessToken);
				using (var result = await client.GetInputStreamAsync(streamUri))
				{
					
					if (!hasConnected)
					{
						Connected?.Invoke();
						hasConnected = true;
					}
					else
					{
						Reconnected?.Invoke();
					}

					using (var s = result.AsStreamForRead())
					{
						await ListensToStreamAsync(s);
					}
				}
			}
#else
			using (HttpClient client = new HttpClient())
			{
				if(hasConnected)
				{
					Reconnecting?.Invoke();
				}
				client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
				using (var stream = await client.GetStreamAsync(streamUri))
				{
					if (ReadTimeout > 0)
					{
						stream.ReadTimeout = ReadTimeout;
					}
					if (!hasConnected)
					{
						Connected?.Invoke();
						hasConnected = true;
					}
					else
					{
						Reconnected?.Invoke();
					}
					await ListensToStreamAsync(stream).ConfigureAwait(true);
				}
			}
#endif
		}

		/// <summary>
		/// Listenses to stream for Web Events
		/// </summary>
		/// <param name="eventStream">The event stream.</param>
		/// <returns>Task that can be awaited</returns>
		protected virtual async Task ListensToStreamAsync(Stream eventStream)
		{
			reader = new StreamReader(eventStream);
			String eventName = null;
			String line;
			List<ParticleEventData> items = new List<ParticleEventData>();
			while (!_stop && !reader.EndOfStream)
			{
				line = await reader.ReadLineAsync();
				if (line?.StartsWith("event:") == true)
				{
					eventName = null;
					String s = line.Substring(6);
					if (!String.IsNullOrWhiteSpace(s))
					{
						eventName = s.Trim();
					}
				}
				else if (line?.StartsWith("data:") == true)
				{
					String s = line.Substring(5);
					if (!String.IsNullOrWhiteSpace(s))
					{
						var d = JsonConvert.DeserializeObject<ParticleEventData>(s);
						if (d?.PublishedAt.HasValue == true) // Convert the time to the local system time if its set
						{
							d.PublishedAt = d.PublishedAt.Value.ToLocalTime();
						}
						items.Add(d);
					}
				}
				else if (String.IsNullOrWhiteSpace(line))
				{
					if (items.Count > 0)
					{
						await Task.Run(() =>
						{
							fireEvent(eventName, items.ToArray());
							items.Clear();
						});
					}
				}
			}
		}

		/// <summary>
		/// Stops listening to the event stream
		/// </summary>
		public void Stop()
		{
			_stop = true;
			hasConnected = false;
		}
	}
}
