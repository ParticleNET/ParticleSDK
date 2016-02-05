using Particle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParticleSDKTests.NUnit
{
	public class ParticleEventManagerMock : ParticleEventManager
	{
		public ParticleEventManagerMock() : base()
		{
		}

		public Task ListensToStreamAsyncMock(Stream s)
		{
			return ListensToStreamAsync(s);
		}

	}
}
