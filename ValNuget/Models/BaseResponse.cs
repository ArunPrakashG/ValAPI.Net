using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ValAPINet.Models {
	public class BaseResponse {
		[JsonProperty]
		public int StatusCode { get; set; }
	}
}
