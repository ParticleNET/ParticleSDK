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
		/// <summary>
		/// Occurs when an event is encountered from the particle cloud api
		/// </summary>
		public event EventHandler<WebEventArgs> Events;

		private async void fireEvent(String eventName, ParticleEventData[] data)
		{
			Events?.Invoke(this, new WebEventArgs
			{
				Event = eventName,
				Data = data
			});
		}

		/// <summary>
		/// Listens to specified event stream.
		/// </summary>
		/// <param name="eventStream">The event stream.</param>
		public async void Listen(Stream eventStream)
		{
			try {
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
			catch(IOException ex)
			{

			}
			catch(ObjectDisposedException ode)
			{

			}
		}

		/// <summary>
		/// Stops listening to the event stream
		/// </summary>
		public void Stop()
		{
			
		}
	}
}
