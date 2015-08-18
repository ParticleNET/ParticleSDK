using Newtonsoft.Json.Linq;
using Particle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParticleSDKTests
{
	public class ParticleDeviceMock : ParticleDevice
	{
		public ParticleDeviceMock(JObject obj): base(new ParticleCloudMock(), obj)
		{

		}

		public ParticleDeviceMock(ParticleCloud cloud, JObject obj) : base(cloud, obj)
		{

		}

		public void ParseObjectMock(JObject obj)
		{
			base.ParseObject(obj);
		}
	}
}
