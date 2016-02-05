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
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Particle
{
	/// <summary>
	/// This class is used to manage the stream of events from the Particle Cloud and Turn them into native .net events
	/// </summary>
	public class ParticleEventManager
	{
		private bool stop = false;
		private StreamReader reader;
		private Uri streamUri;
		/// <summary>
		/// Occurs when an event is encountered from the particle cloud api
		/// </summary>
		public event EventHandler<WebEventArgs> Events;

		/// <summary>
		/// Initializes a new instance of the <see cref="ParticleEventManager"/> class. Without requiring the HttpClient, Access Token, or Uri mostly used for unit tests
		/// </summary>
		protected ParticleEventManager()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParticleEventManager"/> class.
		/// </summary>
		/// <param name="client">The HTTP client used to listen for events.</param>
		/// <param name="streamUri">The stream URI. i.e. https://api.particle.io/v1/devices/events </param>
		/// <param name="accessToken">The access token for authentication.</param>
		public ParticleEventManager(HttpClient client, Uri streamUri, String accessToken)
		{
			this.streamUri = streamUri;
		}

		/// <summary>
		/// Fires the event.
		/// </summary>
		/// <param name="eventName">Name of the event.</param>
		/// <param name="data">The data.</param>
		protected async void fireEvent(String eventName, ParticleEventData[] data)
		{
			Events?.Invoke(this, new WebEventArgs
			{
				Event = eventName,
				Data = data
			});
		}

		/// <summary>
		/// Starts listening to the events coming in from the specified stream
		/// </summary>
		public async void Start()
		{

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
			while (!stop)
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
			stop = true;
		}
	}
}
