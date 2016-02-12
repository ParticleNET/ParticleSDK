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
using NUnit.Framework;
using Particle;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ParticleSDKTests.NUnit
{
	[TestFixture]
	public class ParticleEventManagerTests
	{
		[Test]
		public async Task ListensToStreamAsyncTest()
		{
			ParticleEventManagerMock eventManager = new ParticleEventManagerMock();
			WebEventArgs lastEvent = null;
			int count = 0;
			eventManager.Events += (s, e) =>
			{
				lastEvent = e;
				count++;
				eventManager.Stop();
			};

			using(Stream s = new MemoryStream())
			{
				StreamWriter w = new StreamWriter(s, Encoding.UTF8);
				w.Write(":ok\n\nevent: test\ndata: {\"data\":\"25.34\",\"ttl\":\"60\",\"published_at\":\"2015-07-18T00:12:18.174Z\",\"coreid\":\"0123456789abcdef01234567\"}\n\n");
				w.Flush();
				s.Position = 0; // go back to the beginning of the stream
				await eventManager.ListensToStreamAsyncMock(s);
				await Task.Delay(500); // Delay a little bit so we make sure the other threads has time to execute.


				Assert.IsNotNull(lastEvent);
				Assert.AreEqual("test", lastEvent.Event);
				Assert.IsNotNull(lastEvent.Data);
				Assert.AreEqual(1, lastEvent.Data.Length);
				var data = lastEvent.Data[0];
				Assert.AreEqual("25.34", data.Data);
				Assert.AreEqual(60, data.TimeToLive);
				Assert.AreEqual(DateTime.Parse("2015-07-18T00:12:18.174Z").ToLocalTime(), data.PublishedAt);
				Assert.AreEqual("0123456789abcdef01234567", data.CoreId);
			}
		}

		/* This test does not always execute correctly because the Task To fire the event may not execute until after the Task for ListenToStreamAsync finishes executing

		[Test]
		public async Task ListensToStreamAsyncExceptionTest()
		{
			ParticleEventManagerMock eventManager = new ParticleEventManagerMock();
			WebEventArgs lastEvent = null;
			int count = 0;
			

			using (Stream s = new MemoryStream())
			{
				eventManager.Events += (se, e) =>
				{
					s.Close(); // cause the exception to be throw from the Listen method.
					lastEvent = e;
					count++;
				};

				StreamWriter w = new StreamWriter(s, Encoding.UTF8);
				w.Write(":ok\n\nevent: test\ndata: {\"data\":\"25.34\",\"ttl\":\"60\",\"published_at\":\"2015-07-18T00:12:18.174Z\",\"coreid\":\"0123456789abcdef01234567\"}\n\nevent: test\ndata: {\"data\":\"25.34\",\"ttl\":\"60\",\"published_at\":\"2015-07-18T00:12:18.174Z\",\"coreid\":\"0123456789abcdef01234567\"}\n\n");
				w.Flush();
				s.Position = 0; // go back to the beginning of the stream
				bool exceptionThrown = false;
				try
				{
					await eventManager.ListensToStreamAsyncMock(s);
				}
				catch(Exception ex)
				{
					Assert.IsTrue(ex is ObjectDisposedException);
					exceptionThrown = true;
				}
				Assert.IsTrue(exceptionThrown);
				
			}
		}*/

		[Test]
		public async Task ListenForPublicEventsTest()
		{
			var eventManager = new ParticleEventManager(new Uri("https://api.particle.io/v1/events"), System.Environment.GetEnvironmentVariable("ParticleAccessToken"));
			long eventCount = 0;
			eventManager.Events += (s, e) =>
			{
				eventCount++;
				Assert.IsFalse(String.IsNullOrWhiteSpace(e.Event));
				Assert.IsNotNull(e.Data);
				Assert.IsNotNull(e.Data.Length > 0);
			};

			eventManager.Start();
			Assert.IsTrue(eventManager.IsRunning);
			await Task.Delay(5000);
			eventManager.Stop();
			Assert.IsFalse(eventManager.IsRunning);

			Assert.IsTrue(eventCount > 0); // Its possible for this to be 0 but not very likely

		}

		/* It would be nice to have a test for all the events the Event Manager has but not sure how to simulate the network disconnecting and reconnecting that would be required */
	}
}
