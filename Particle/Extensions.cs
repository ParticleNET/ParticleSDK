using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Particle
{
	public static class Extensions
	{
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
