﻿/*
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Particle
{
	/// <summary>
	/// Extensions the ParicleSDK uses
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// Runs the provided <paramref name="action"/> on the SynchronizationContext if one is provided otherwise runs the action on the current thread
		/// </summary>
		/// <param name="context">The SynchronizationContext.</param>
		/// <param name="action">The action to run</param>
		public static void InvokeIfRequired(this SynchronizationContext context, Action action)
		{
			if(context == null)
			{
				action();
				return;
			}

			if (SynchronizationContext.Current == context)
			{
				action();
			}
			else
			{
				context.Post(new SendOrPostCallback((t)=> { action(); }), context); // send = synchronously
																	// context.Post(action)  - post is asynchronous.
			}
		}
	}
}
