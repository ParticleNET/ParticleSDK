/*
Copyright 2015 ParticleNET

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

		public override Task<RequestResponse> MakeDeleteRequestAsync(string method)
		{
			return Task.Run<RequestResponse>(() =>
			{
				if (RequestCallBack != null)
				{
					return RequestCallBack("DELETE", method, null);
				}

				throw new NullReferenceException("Please provide a RequestCallBack for this test");
			});
		}
	}
}
