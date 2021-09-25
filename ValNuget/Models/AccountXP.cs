using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ValAPINet.Models;

namespace ValAPINet {
	public class Progress {
		public int Level { get; set; }
		public int XP { get; set; }
	}

	public class XPSource {
		public string ID { get; set; }
		public int Amount { get; set; }
	}

	public class XpHistory {
		public string ID { get; set; }
		public DateTime MatchStart { get; set; }
		public Progress StartProgress { get; set; }
		public Progress EndProgress { get; set; }
		public int XPDelta { get; set; }
		public List<XPSource> XPSources { get; set; }
		public List<object> XPMultipliers { get; set; }
	}

	public class AccountXP : BaseResponse {
		public int Version { get; set; }
		public string Subject { get; set; }
		public Progress Progress { get; set; }
		public List<XpHistory> History { get; set; }
		public string LastTimeGrantedFirstWin { get; set; }
		public string NextTimeFirstWinAvailable { get; set; }

		[JsonConstructor]
		public AccountXP() { }

		public static async Task<AccountXP> GetOffers(ValorantClient client) => await client.RequestAsync<AccountXP>(new HttpRequestMessage(HttpMethod.Get, $"https://pd.{Client.AuthorizationHandler.Region}.a.pvp.net/account-xp/v1/players/{Client.AuthorizationHandler.Subject}"), true).ConfigureAwait(false);
	}
}
