﻿using NUnit.Framework;
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

		[Test]
		public void ListensToStreamAsyncExceptionTest()
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
				Assert.Throws<ObjectDisposedException>(async () => await eventManager.ListensToStreamAsyncMock(s));
				
			}
		}
	}
}
