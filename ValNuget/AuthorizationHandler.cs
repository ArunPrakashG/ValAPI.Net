using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ValAPINet.Models;

namespace ValAPINet {
	internal class AuthorizationHandler {
		private const string AuthorizationBaseUrl = "https://auth.riotgames.com/api/v1/authorization";
		private const string EntitlementBaseUrl = "https://entitlements.auth.riotgames.com/api/token/v1";
		private const string VersionBaseUrl = "https://valorant-api.com/v1/version";

		internal string EntitlementToken { get; private set; }
		internal string AccessToken { get; private set; }
		internal string Subject { get; private set; }

		internal string ClientVersion { get; private set; }
		internal Region Region { get; private set; }

		internal ValorantVersion Version { get; private set; }

		private readonly HttpClient Client;
		private readonly LoginInfo LoginInfo;

		internal AuthorizationHandler(HttpClient client, LoginInfo loginInfo) {
			Client = client;
			LoginInfo = loginInfo;
		}

		internal async Task<bool> Authorize() {
			return await AuthorizeClient() && await GetEntitlements() && await FetchVersion();

		}

		private async Task<bool> AuthorizeClient() {
			using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, AuthorizationBaseUrl)) {
				request.Content = new StringContent("{\"client_id\":\"play-valorant-web-prod\",\"nonce\":\"1\",\"redirect_uri\":\"https://playvalorant.com/opt_in" + "\",\"response_type\":\"token id_token\",\"scope\":\"account openid\"}", Encoding.UTF8, "application/json");
				using (HttpResponseMessage response = await Client.SendAsync(request).ConfigureAwait(false)) {
					if (response.StatusCode != HttpStatusCode.OK) {
						return false;
					}
				}
			}

			using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, AuthorizationBaseUrl)) {
				request.Content = new StringContent("{\"type\":\"auth\",\"username\":\"" + LoginInfo.Username + "\",\"password\":\"" + LoginInfo.Password + "\"}", Encoding.UTF8, "application/json");
				using (HttpResponseMessage response = await Client.SendAsync(request).ConfigureAwait(false)) {
					if (response.StatusCode != HttpStatusCode.OK) {
						return false;
					}

					var authJsonString = await response.Content.ReadAsStringAsync();

					if (authJsonString.Contains("auth_failure")) {
						return false;
					}

					var authJson = JsonConvert.DeserializeObject(authJsonString);
					JToken authObj = JObject.FromObject(authJson);

					string authURL = authObj["response"]["parameters"]["uri"].Value<string>();
					var access_tokenVar = Regex.Match(authURL, @"access_token=(.+?)&scope=").Groups[1].Value;
					AccessToken = access_tokenVar;
					return true;
				}
			}
		}

		private async Task<bool> GetEntitlements() {
			using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, EntitlementBaseUrl)) {
				request.Headers.Add("Authorization", $"Bearer {AccessToken}");
				request.Content = new StringContent("{}", Encoding.UTF8, "application/json");
				using (HttpResponseMessage response = await Client.SendAsync(request).ConfigureAwait(false)) {
					if (response.StatusCode != HttpStatusCode.OK) {
						return false;
					}

					var entitlementJson = await response.Content.ReadAsStringAsync();
					var entitlement_token = JsonConvert.DeserializeObject(entitlementJson);
					JToken entitlement_tokenObj = JObject.FromObject(entitlement_token);

					EntitlementToken = entitlement_tokenObj["entitlements_token"].Value<string>();
					AccessToken = AccessToken;
					EntitlementToken = EntitlementToken;
					var jsonWebToken = new JsonWebToken(AccessToken);
					Subject = jsonWebToken.Subject;
				}
			}

			return false;
		}

		private async Task<bool> FetchVersion() {
			using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, VersionBaseUrl)) {
				using (HttpResponseMessage response = await Client.SendAsync(request).ConfigureAwait(false)) {
					if (response.StatusCode != HttpStatusCode.OK) {
						return false;
					}

					Version = JsonConvert.DeserializeObject<ValorantVersion>(await response.Content.ReadAsStringAsync());
					ClientVersion = $"{Version.VersionContainer.Branch}-shipping-{Version.VersionContainer.BuildVersion}-{Version.VersionContainer.Version.Substring(Version.VersionContainer.Version.Length - 6)}";
					return true;
				}
			}
		}
	}

}
