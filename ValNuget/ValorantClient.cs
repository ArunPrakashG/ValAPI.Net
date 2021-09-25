using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ValAPINet {
	public class ValorantClient {
		private readonly HttpClientHandler ClientHandler;
		private readonly CookieContainer Cookies;
		private readonly LoginInfo LoginInfo;
		private readonly HttpClient Client;		
		private readonly SemaphoreSlim RequestSync;
		internal readonly AuthorizationHandler AuthorizationHandler;

		public bool Initialized { get; private set; }

		public ValorantClient(LoginInfo loginInfo,  IWebProxy proxy = null) {
			Cookies = new CookieContainer();
			LoginInfo = loginInfo;
			RequestSync = new SemaphoreSlim(1, 1);
			ClientHandler = new HttpClientHandler() {
				CookieContainer = Cookies,
				UseCookies = true,
				Proxy = proxy,
				UseProxy = proxy != null,
			};

			Client = new HttpClient(ClientHandler);
			AuthorizationHandler = new AuthorizationHandler(Client, LoginInfo);
		}

		public async Task InitializeClient(AuthorizationMethod method) {
			if (Initialized) {
				return;
			}

			Initialized = await AuthorizationHandler.Authorize();
		}

		internal async Task<T> RequestAsync<T>(HttpRequestMessage request, bool authorizeRequest = false) {
			await RequestSync.WaitAsync().ConfigureAwait(false);

			try {
				if (authorizeRequest && Initialized) {
					request.Headers.Add("Authorization", $"Bearer {AuthorizationHandler.AccessToken}");
					request.Headers.Add("X-Riot-Entitlements-JWT", $"{AuthorizationHandler.EntitlementToken}");
					request.Headers.Add("X-Riot-ClientPlatform", "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
					request.Headers.Add("X-Riot-ClientVersion", $"{AuthorizationHandler.ClientVersion}");
				}

				using (request) {
					using(var response = await Client.SendAsync(request).ConfigureAwait(false)) {
						if(response.StatusCode != HttpStatusCode.OK) {
							return default;
						}

						return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
					}
				}
			}
			finally {
				RequestSync.Release();
			}
		}

		public enum AuthorizationMethod {
			Local,
			Network,
		}
	}
}
