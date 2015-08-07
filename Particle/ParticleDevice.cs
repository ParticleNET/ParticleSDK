/*
Copyright 2015 Sannel Software, L.L.C.

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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Particle
{
	public class ParticleDevice : ParticleBase
	{
		private ParticleCloud cloud;
		private String id;
		public String Id
		{
			get { return id; }
			internal set { SetProperty(ref id, value); }
		}

		private String name;
		public String Name
		{
			get { return name; }
			internal set { SetProperty(ref name, value); }
		}

		private String lastApp;
		public String LastApp
		{
			get { return lastApp; }
			internal set { SetProperty(ref lastApp, value); }
		}

		private String lastIPAddress;
		public String LastIPAddress
		{
			get { return lastIPAddress; }
			internal set { SetProperty(ref lastIPAddress, value); }
		}

		private DateTime? lastHeard;
		public DateTime? LastHeard
		{
			get { return lastHeard; }
			internal set { SetProperty(ref lastHeard, value); }
		}

		private ParticleDeviceType deviceType;
		public ParticleDeviceType DeviceType
		{
			get { return deviceType; }
			internal set { SetProperty(ref deviceType, value); }
		}

		private bool connected;
		public bool Connected
		{
			get { return connected; }
			internal set { SetProperty(ref connected, value); }
		}

		private ObservableCollection<Variable> variables = new ObservableCollection<Variable>();
		public ObservableCollection<Variable> Variables
		{
			get { return variables; }
		}

		internal protected ParticleDevice(ParticleCloud cloud, JObject obj)
		{
			if (cloud == null)
			{
				throw new ArgumentNullException(nameof(cloud));
			}
			this.cloud = cloud;
			ParseObject(obj);
		}

		private String parseStringValue(JToken token)
		{
			if (token?.Type == JTokenType.String)
			{
				return token.Value<String>();
			}
			return null;
		}

		private DateTime? parseDateTimeValue(JToken token)
		{
			if (token?.Type == JTokenType.Date)
			{
				return token.Value<DateTime>();
			}

			return null;
		}

		private int parseIntValue(JToken token)
		{
			if (token?.Type == JTokenType.Integer)
			{
				return (int)token.Value<long>();
			}
			else if (token?.Type == JTokenType.Float)
			{
				return (int)token.Value<double>();
			}

			return 0;
		}

		private bool parseBooleanValue(JToken token)
		{
			if (token?.Type == JTokenType.Boolean)
			{
				return token.Value<Boolean>();
			}
			return false;
		}

		protected virtual void ParseVariables(JObject obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}

			ParticleCloud.SyncContext.InvokeIfRequired(() =>
			{
				variables.Clear();

				foreach (var prop in obj.Properties())
				{
					var variable = new Variable(this)
					{
						Name = prop.Name,
						Type = prop.Value?.ToString()
					};
					variables.Add(variable);
				}
			});
		}

		protected virtual void ParseFunctions(JArray arr)
		{
			if (arr == null)
			{
				throw new ArgumentNullException(nameof(arr));
			}

			functions.Clear();

			foreach (var i in arr)
			{
				if (i.Type == JTokenType.String)
				{
					functions.Add(i.Value<String>());
				}
			}
		}

		protected virtual void ParseObject(JObject obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}

			foreach (var prop in obj.Properties())
			{
				var name = prop.Name;
				switch (name)
				{
					case "id":
						Id = parseStringValue(prop.Value);
						break;
					case "name":
						Name = parseStringValue(prop.Value);
						break;
					case "last_app":
						LastApp = parseStringValue(prop.Value);
						break;
					case "last_ip_address":
						LastIPAddress = parseStringValue(prop.Value);
						break;
					case "last_heard":
						LastHeard = parseDateTimeValue(prop.Value);
						break;
					case "product_id":
						DeviceType = (ParticleDeviceType)parseIntValue(prop.Value);
						break;
					case "connected":
						Connected = parseBooleanValue(prop.Value);
						break;
					case "variables":
						if (prop.Value?.Type == JTokenType.Object)
						{
							ParseVariables((JObject)prop.Value);

						}
						break;
					case "functions":
						if (prop.Value?.Type == JTokenType.Array)
						{
							ParseFunctions((JArray)prop.Value);
						}
						break;
				}
			}
		}

		private List<String> functions = new List<string>();
		public IReadOnlyList<String> Functions
		{
			get
			{
				return functions;
			}
		}

		public async Task<Result<T>> GetVariableValueAsync<T>(String variableName)
		{
			throw new NotImplementedException();
		}

		public async Task<Result<int>> CallFunctionAsync(String functionName, params String[] args)
		{
			throw new NotImplementedException();
		}

		public async Task<Result> RefreshAsync()
		{
			var response = await cloud.MakeGetRequestAsync($"devices/{Id}");
			if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
			{
				await cloud.RefreshTokenAsync();
				response = await cloud.MakeGetRequestAsync($"devices/{Id}");
				if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
				{
					return response.AsResult();
				}
			}

			if (response.StatusCode == System.Net.HttpStatusCode.OK)
			{
				if (response.Response?.Type == JTokenType.Object)
				{
					await Task.Run(() => ParseObject((JObject)response.Response));
					return new Result(true);
				}
				else
				{
					return new Result()
					{
						Error = Messages.UnexpectedResponse,
						ErrorDescription = response.Response?.ToString()
					};
				}
			}
			else
			{
				return response.AsResult();
			}
		}

		public async Task<Result> UnclaimAsync()
		{
			throw new NotImplementedException();
		}

		public async Task<Result> RenameAsync()
		{
			throw new NotImplementedException();
		}

		// this method signature should probably change
		public async Task<Result> FlashFilesAsync(IDictionary<String, byte[]> files)
		{
			throw new NotImplementedException();
		}

		// this method signature should probably change
		public async Task<Result> FlashKnownAppAsync(String appName)
		{
			throw new NotImplementedException();
		}

		// this method signature should probably change
		public async Task<Result> CompileAndFlashFiles(IDictionary<String, byte[]> files)
		{
			throw new NotImplementedException();
		}

		// this method signature should probably change and its return type will change
		public async Task<Result> ComplileFilesAsync(IDictionary<String, byte[]> files)
		{
			throw new NotImplementedException();
		}

		/*id: "390032000647343232363230"
name: "Proto"
last_app: null
last_ip_address: "174.52.197.239"
last_heard: "2015-07-11T05:25:09.960Z"
product_id: 6
connected: true*/

		/*
id: "390032000647343232363230"
name: "Proto"
connected: true
variables: {
temp: "double"
}-
functions: [1]
0:  "led"
-
cc3000_patch_version: null
product_id: 6
last_heard: "2015-07-11T05:32:56.614Z"*/
	}
}
