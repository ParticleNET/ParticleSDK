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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Particle
{
	/// <summary>
	/// Represents the results from SignupWithUserAsync
	/// </summary>
	public class CreateUserResult
	{
		/// <summary>
		/// Gets or sets a value indicating whether the user was created.
		/// </summary>
		/// <value>
		///   <c>true</c> if ok; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("ok")]
		public bool Ok { get; set; }

		/// <summary>
		/// Gets or sets the errors.
		/// </summary>
		/// <value>
		/// The errors.
		/// </value>
		[JsonProperty("errors")]
		public String[] Errors { get; set; }
		/*{
  "ok": false,
  "errors": [
	"username must be an email address"
  ]
}*/
	}
}
