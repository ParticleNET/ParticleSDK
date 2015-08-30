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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Particle
{
	/// <summary>
	/// Represents a Device like the Core, Photon or Electron
	/// </summary>
	public class ParticleDevice : ParticleBase
	{
		private ParticleCloud cloud;
		private String id;
		/// <summary>
		/// Gets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		public String Id
		{
			get { return id; }
			internal set { SetProperty(ref id, value); }
		}

		private String name;
		/// <summary>
		/// The name of the core
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public String Name
		{
			get { return name; }
			internal set { SetProperty(ref name, value); }
		}

		private String lastApp;
		/// <summary>
		/// Gets the last application flashed to the core
		/// </summary>
		/// <value>
		/// The last application.
		/// </value>
		public String LastApp
		{
			get { return lastApp; }
			internal set { SetProperty(ref lastApp, value); }
		}

		private String lastIPAddress;
		/// <summary>
		/// Gets the last IP address the Device was connected with
		/// </summary>
		/// <value>
		/// The last IP address.
		/// </value>
		public String LastIPAddress
		{
			get { return lastIPAddress; }
			internal set { SetProperty(ref lastIPAddress, value); }
		}

		private DateTime? lastHeard;
		/// <summary>
		/// Gets the last heard date.
		/// </summary>
		/// <value>
		/// The last heard.
		/// </value>
		public DateTime? LastHeard
		{
			get { return lastHeard; }
			internal set { SetProperty(ref lastHeard, value); }
		}

		private ParticleDeviceType deviceType;
		/// <summary>
		/// Gets the type of the device.
		/// </summary>
		/// <value>
		/// The type of the device.
		/// </value>
		public ParticleDeviceType DeviceType
		{
			get { return deviceType; }
			internal set { SetProperty(ref deviceType, value); }
		}

		private bool connected;
		/// <summary>
		/// Gets a value indicating whether this <see cref="ParticleDevice"/> is connected.
		/// </summary>
		/// <value>
		///   <c>true</c> if connected; otherwise, <c>false</c>.
		/// </value>
		public bool Connected
		{
			get { return connected; }
			internal set { SetProperty(ref connected, value); }
		}

		private ObservableCollection<Variable> variables = new ObservableCollection<Variable>();
		/// <summary>
		/// Gets the variables defined for this device. Be sure to call <see cref="RefreshAsync"/> to refresh this list.
		/// </summary>
		/// <value>
		/// The variables.
		/// </value>
		public ObservableCollection<Variable> Variables
		{
			get { return variables; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParticleDevice"/> class.
		/// </summary>
		/// <param name="cloud">The cloud.</param>
		/// <param name="obj">The JSon object to parse</param>
		/// <exception cref="System.ArgumentNullException"></exception>
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
				return token.Value<DateTime>().ToLocalTime();
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

		/// <summary>
		/// Parses the variables.
		/// </summary>
		/// <param name="obj">The object.</param>
		protected virtual void ParseVariables(JObject obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}

			var list = new List<Variable>();

			foreach (var prop in obj.Properties())
			{
				var first = variables.FirstOrDefault(i => String.Compare(i.Name, prop.Name) == 0);
				if (first == null)
				{
					first = new Variable(this)
					{
						Name = prop.Name,
						Type = prop.Value?.ToString()
					};
				}
				else
				{
					first.Type = prop.Value?.ToString();
				}

				list.Add(first);
			}

			ParticleCloud.SyncContext.InvokeIfRequired(() =>
			{
				variables.Clear();

				foreach (var item in list)
				{
					variables.Add(item);
				}
			});
		}

		/// <summary>
		/// Parses the functions.
		/// </summary>
		/// <param name="arr">The arr.</param>
		protected virtual void ParseFunctions(JArray arr)
		{
			if (arr == null)
			{
				throw new ArgumentNullException(nameof(arr));
			}

			ParticleCloud.SyncContext.InvokeIfRequired(() =>
			{
				functions.Clear();

				foreach (var i in arr)
				{
					if (i.Type == JTokenType.String)
					{
						functions.Add(i.Value<String>());
					}
				}
			});
		}

		/// <summary>
		/// Parses the JSon Object representing a particle device
		/// </summary>
		/// <param name="obj">The object.</param>
		protected internal virtual void ParseObject(JObject obj)
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
						ParticleCloud.SyncContext.InvokeIfRequired(() =>
						{
							Id = parseStringValue(prop.Value);
						});
						break;
					case "name":
						ParticleCloud.SyncContext.InvokeIfRequired(() =>
						{
							Name = parseStringValue(prop.Value);
						});
						break;
					case "last_app":
						ParticleCloud.SyncContext.InvokeIfRequired(() =>
						{
							LastApp = parseStringValue(prop.Value);
						});
						break;
					case "last_ip_address":
						ParticleCloud.SyncContext.InvokeIfRequired(() =>
						{
							LastIPAddress = parseStringValue(prop.Value);
						});
						break;
					case "last_heard":
						ParticleCloud.SyncContext.InvokeIfRequired(() =>
						{
							LastHeard = parseDateTimeValue(prop.Value);
						});
						break;
					case "product_id":
						ParticleCloud.SyncContext.InvokeIfRequired(() =>
						{
							DeviceType = (ParticleDeviceType)parseIntValue(prop.Value);
						});
						break;
					case "connected":
						ParticleCloud.SyncContext.InvokeIfRequired(() =>
						{
							Connected = parseBooleanValue(prop.Value);
						});
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

		private ObservableCollection<String> functions = new ObservableCollection<string>();
		/// <summary>
		/// Gets the functions for this device. Be sure to call <see cref="RefreshAsync"/> to refresh this list.
		/// </summary>
		/// <value>
		/// The functions.
		/// </value>
		public IReadOnlyList<String> Functions
		{
			get
			{
				return functions;
			}
		}

		/// <summary>
		/// Gets the variable value asynchronous for the provided <paramref name="variable"/> name.
		/// </summary>
		/// <param name="variable">The variable.</param>
		/// <returns></returns>
		public async Task<Result<Variable>> GetVariableValueAsync(String variable)
		{
			if (variable == null)
			{
				throw new ArgumentNullException(nameof(variable));
			}

			Variable vari;
			vari = await Task.Run(() => variables.FirstOrDefault(i => String.Compare(i.Name, variable) == 0));
			if (vari == null)
			{
				vari = new Variable(this);
				vari.Name = variable;
			}

			return await GetVariableValueAsync(vari);
		}

		/// <summary>
		/// Gets the variable value asynchronous for the provided <paramref name="variable"/>.
		/// </summary>
		/// <param name="variable">The variable.</param>
		/// <returns></returns>
		public async Task<Result<Variable>> GetVariableValueAsync(Variable variable)
		{

			if (variable == null)
			{
				throw new ArgumentNullException(nameof(variable));
			}

			if (String.IsNullOrWhiteSpace(variable.Name))
			{
				throw new ArgumentException(Messages.PassedVariableMustHaveAName, nameof(variable));
			}

			var svariable = await Task.Run(() => variables.FirstOrDefault(i => i == variable));

			if (svariable == null)
			{
				ParticleCloud.SyncContext.InvokeIfRequired(() =>
				{
					variables.Add(variable);
				});
			}

			var response = await cloud.MakeGetRequestAsync($"devices/{Id}/{Uri.EscapeDataString(variable.Name)}");
			if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
			{
				await cloud.RefreshTokenAsync();
				response = await cloud.MakeGetRequestAsync($"devices/{Id}/{Uri.EscapeDataString(variable.Name)}");
				if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
				{
					return response.AsResult<Variable>();
				}
			}

			if (response.StatusCode == System.Net.HttpStatusCode.OK)
			{
				var tresult = response.Response.SelectToken("result");
				variable.Value = tresult.Value<Object>().ToString();
				/*
			"name": "temperature",
  "result": 46,
  "coreInfo": {
	"name": "weatherman",
	"deviceID": "0123456789abcdef01234567",
	"connected": true,
	"last_handshake_at": "2015-07-17T22:28:40.907Z",
	"last_app": ""
  }*/

				return new Result<Variable>
				{
					Success = true,
					Data = variable
				};
			}
			else
			{
				return response.AsResult<Variable>();
			}
		}

		/// <summary>
		/// Calls the function asynchronous.
		/// </summary>
		/// <param name="functionName">Name of the function.</param>
		/// <param name="arg">The argument.</param>
		/// <returns></returns>
		public async Task<Result<int>> CallFunctionAsync(String functionName, String arg)
		{
			if (String.IsNullOrWhiteSpace(functionName))
			{
				throw new ArgumentNullException(nameof(functionName));
			}

			var response = await cloud.MakePostRequestWithAuthTestAsync($"devices/{Id}/{Uri.EscapeUriString(functionName)}", new KeyValuePair<string, string>("arg", arg));

			if (response.StatusCode == System.Net.HttpStatusCode.OK)
			{
				var returnValue = response.Response.SelectToken("return_value");
				return new Result<int>(true, (int)returnValue.Value<long>());
			}
			else
			{
				return response.AsResult<int>();
			}
		}

		/// <summary>
		/// Refreshes the device from the cloud.
		/// </summary>
		/// <returns></returns>
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

		/// <summary>
		/// Unclaims the Device asynchronous.
		/// </summary>
		/// <returns></returns>
		public async Task<Result> UnclaimAsync()
		{
			var result = await cloud.MakeDeleteRequestWithAuthTestAsync($"devices/{Id}");
			return result.AsResult();
		}

		/// <summary>
		/// Renames the Device.
		/// </summary>
		/// <param name="newName">The new name.</param>
		/// <returns></returns>
		public async Task<Result> RenameAsync(String newName)
		{
			if (String.IsNullOrWhiteSpace(newName))
			{
				throw new ArgumentNullException(nameof(newName));
			}

			var result = await cloud.MakePutRequestWithAuthTestAsync($"devices/{Id}", new KeyValuePair<string, string>("name", newName));
			if (result.StatusCode == System.Net.HttpStatusCode.OK)
			{
				var r = result.AsResult();
				if (String.IsNullOrWhiteSpace(r.Error))
				{
					r.Success = true;
					ParticleCloud.SyncContext.InvokeIfRequired(() =>
					{
						Name = newName;
					});
				}

				return r;
			}
			else
			{
				return result.AsResult();
			}
		}

		/// <summary>
		/// Flashes a known application to a device.
		/// </summary>
		/// <param name="appName">Name of the application.</param>
		/// <returns></returns>
		public async Task<Result> FlashKnownAppAsync(String appName)
		{
			if (String.IsNullOrWhiteSpace(appName))
			{
				throw new ArgumentNullException(nameof(appName));
			}

			var result = await cloud.MakePutRequestWithAuthTestAsync($"devices/{Id}", new KeyValuePair<string, string>("app", appName));
			if(result.StatusCode == System.Net.HttpStatusCode.OK)
			{
				var r = result.AsResult();
				if (String.IsNullOrWhiteSpace(r.Error))
				{
					r.Success = true;
				}
				return r;
			}
			else
			{
				return result.AsResult();
			}
			/*
{
  "id": "310049000647343339373536",
  "status": "Update started"
}
*/
			/*
			{
			  "ok": false,
			  "code": 500,
			  "errors": [
				"Can't flash unknown app tinke"
			  ]
			}*/
		}

					// this method signature should probably change
					/*public async Task<Result> FlashFilesAsync(IDictionary<String, byte[]> files)
					{
						throw new NotImplementedException();
					}*/



			// this method signature should probably change
			/*public async Task<Result> CompileAndFlashFiles(IDictionary<String, byte[]> files)
			{
				throw new NotImplementedException();
			}*/

			/*id: "00000"
	name: "Proto"
	last_app: null
	last_ip_address: "174.33.197.239"
	last_heard: "2015-07-11T05:25:09.960Z"
	product_id: 6
	connected: true*/

			/*
	id: "00000"
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
