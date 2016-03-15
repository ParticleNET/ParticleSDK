using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Particle
{
	/// <summary>
	/// Represents the Json data from a particle Event
	/// </summary>
	public class ParticleEventData
	{
		/// <summary>
		/// Gets or sets the data for the event
		/// </summary>
		/// <value>
		/// The data.
		/// </value>
		[JsonProperty("data")]
		public String Data { get; set; }

		/// <summary>
		/// Gets or sets the time to live.
		/// </summary>
		/// <value>
		/// The time to live.
		/// </value>
		[JsonProperty("ttl")]
		public long? TimeToLive { get; set; }
		/// <summary>
		/// Gets or sets the event was published at.
		/// </summary>
		/// <value>
		/// The published date
		/// </value>
		[JsonProperty("published_at")]
		public DateTime? PublishedAt { get; set; }
		/// <summary>
		/// Gets or sets the core identifier.
		/// </summary>
		/// <value>
		/// The core identifier.
		/// </value>
		[JsonProperty("coreid")]
		public String CoreId { get; set; }

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance as Json data
		/// </summary>
		/// <returns>
		/// A <see cref="System.String" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return JsonConvert.SerializeObject(this);
		}
	}
}
