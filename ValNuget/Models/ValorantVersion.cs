using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ValAPINet.Models {
	internal class ValorantVersion {
		[JsonProperty("status")]
		internal int StatusCode { get; set; }

		[JsonProperty("data")]
		internal VersionContainer VersionContainer { get; set; }
	}

	internal class VersionContainer {
		[JsonProperty("manifestId")]
		internal string ManifestID { get; set; }

		[JsonProperty("branch")]
		internal string Branch { get; set; }

		[JsonProperty("version")]
		internal string Version { get; set; }

		[JsonProperty("buildVersion")]
		internal string BuildVersion { get; set; }

		[JsonProperty("riotClientVersion")]
		internal string RiotClientVersion { get; set; }

		[JsonProperty("buildDate")]
		internal DateTime BuildDate { get; set; }
	}
}
