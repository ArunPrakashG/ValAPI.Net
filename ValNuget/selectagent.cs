﻿using RestSharp;

namespace ValAPINet
{
    public class SelectAgent
    {
        public static void ChooseAgent(AuthorizationHandler au, string matchid, string agent)
        {
            SelectAgent ret = new SelectAgent();

            string url = "https://glz-" + au.Region + "-1." + au.Region + ".a.pvp.net/pregame/v1/matches/" + matchid + "/select/" + agent;
            RestClient client = new RestClient(url);
            client.CookieContainer = au.Cookies;
            //client.CookieContainer = new CookieContainer();

            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", $"Bearer {au.AccessToken}");
            request.AddHeader("X-Riot-Entitlements-JWT", $"{au.EntitlementToken}");
            request.AddHeader("X-Riot-ClientPlatform", $"ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
            request.AddHeader("X-Riot-ClientVersion", $"{au.ClientVersion}");

            string responce = client.Execute(request).Content;
            return;
        }
        public static void LockAgent(AuthorizationHandler au, string matchid, string agent)
        {
            SelectAgent ret = new SelectAgent();

            string url = "https://glz-" + au.Region + "-1." + au.Region + ".a.pvp.net/pregame/v1/matches/" + matchid + "/lock/" + agent;
            RestClient client = new RestClient(url);
            client.CookieContainer = au.Cookies;
            //client.CookieContainer = new CookieContainer();

            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", $"Bearer {au.AccessToken}");
            request.AddHeader("X-Riot-Entitlements-JWT", $"{au.EntitlementToken}");
            request.AddHeader("X-Riot-ClientPlatform", $"ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
            request.AddHeader("X-Riot-ClientVersion", $"{au.ClientVersion}");

            string responce = client.Execute(request).Content;
            return;
        }
    }
}
