using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;

namespace NotoIto.App.WatchDogsVRC.VRCWebAPI
{
    public class UserFriendList
    {
        private const string APIBaseUrl = "https://api.vrchat.cloud/api/1";
        private string authToken;
        private string apiKey;
        public UserFriendList(string user,string password)
        {
            using(var client = new HttpClient())
            {
                var resp = client.GetAsync(APIBaseUrl + "/config").Result;
                var content = resp.Content.ReadAsStringAsync().Result;
                var contentObject = JsonConvert.DeserializeObject<dynamic>(content);
                apiKey = contentObject["clientApiKey"];
            }
            auth(user, password);
        }


        public List<string> Get()
        {
            List<string> returnList = new List<string>();
            using (var client = new HttpClient())
            {
                var parameters = new Dictionary<string, string>()
                    {
                        { "apiKey", apiKey },
                        {"authToken", authToken }
                    };
                var resp = client.GetAsync(APIBaseUrl + $"/auth/user/friends?{ new FormUrlEncodedContent(parameters).ReadAsStringAsync().Result }").Result;
                var content = resp.Content.ReadAsStringAsync().Result;
                var friends = JsonConvert.DeserializeObject<dynamic>(content);
                foreach(var f in friends)
                {
                    returnList.Add(f["displayName"].ToString());
                }
            }
            return returnList;
        }

        private void auth(string id,string password)
        {
            var parameters = new Dictionary<string, string>()
                {
                    { "apiKey", apiKey },
                };
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(APIBaseUrl + $"/auth/user?{ new FormUrlEncodedContent(parameters).ReadAsStringAsync().Result }")
            };

            // Basic認証ヘッダを付与する
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", id, password))));
            CookieContainer cc = new CookieContainer();

            HttpClientHandler hcHandler = new HttpClientHandler();
            hcHandler.UseCookies = true;
            hcHandler.CookieContainer = cc;
            using (var client = new HttpClient(hcHandler))
            {
                var resp = client.SendAsync(request).Result;
                if (resp.StatusCode != HttpStatusCode.OK)
                    throw new VRCAuthorizationException();
            }
            var c = cc.GetCookies(new Uri(APIBaseUrl + "/auth/user"));
            authToken = c[0].Value;
        }
    }

    public class VRCAuthorizationException: Exception { }
}
