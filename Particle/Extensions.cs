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
