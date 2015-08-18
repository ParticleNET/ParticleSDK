using Particle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ParticleSDKTests
{
	public class ParticleCloudMock : ParticleCloud
	{
		public ParticleCloudMock()
			: base()
		{

		}

		public Func<String, String, KeyValuePair<String, String>[], RequestResponse> RequestCallBack
		{
			get;
			set;
		}

		public override Task<RequestResponse> MakeGetRequestAsync(string method)
		{
			return Task.Run<RequestResponse>(() =>
				{
					if (RequestCallBack != null)
					{
						return RequestCallBack("GET", method, null);
					}

					throw new NullReferenceException("Please provide a RequestCallBack for this test");
				});
		}

		public override Task<RequestResponse> MakePostRequestAsync(string method, params KeyValuePair<string, string>[] arguments)
		{
			return Task.Run<RequestResponse>(() =>
			{
				if (RequestCallBack != null)
				{
					return RequestCallBack("POST", method, arguments);
				}

				throw new NullReferenceException("Please provide a RequestCallBack for this test");
			});
		}
	}
}
