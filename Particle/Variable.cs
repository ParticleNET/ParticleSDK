using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Particle
{
	public class Variable : ParticleBase
	{
		private ParticleDevice device;

		public Variable(ParticleDevice device)
		{
			this.device = device;
		}

		private String name;
		public String Name
		{
			get { return name; }
			internal set { SetProperty(ref name, value); }
		}

		private String type;
		public String Type
		{
			get { return type; }
			internal set { SetProperty(ref type, value); }
		}

		private String value;
		public String Value
		{
			get { return value; }
			internal set { SetProperty(ref this.value, value); }
		}

		public async Task<Result> RefreshValueAsync()
		{
			throw new NotImplementedException();
		}
	}
}
