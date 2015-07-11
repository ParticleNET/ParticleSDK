using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Particle
{
	public struct LoginResult
	{
		public bool IsAuthenticated { get; set; }
		public String Error { get; set; }
	}
}
