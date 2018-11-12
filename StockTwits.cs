using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StockTwits
{
    public class StockTwitsClient
    {
        private const string AUTH_URL = "https://api.stocktwits.com/api/2/oauth/token";
        private const string CREATE_URL = "https://api.stocktwits.com/api/2/messages/create.json";


        public async Task<string> Authorize(string clientid, string clientsecret, string code, string redirectUri)
        {
            var postData =
                $"client_id={clientid}&client_secret={clientsecret}&code={code}&grant_type=authorization_code&redirect_uri={redirectUri}";

            return await SendRequest(AUTH_URL, postData);
        }

        public async Task CreateMessage(string token, string body)
        {
            await SendRequest($"{CREATE_URL}?access_token={token}", $"body={body}");
        }

        public async Task CreateMessageWithChart(string token, string body, string filepath)
        {
            var form = new MultipartFormDataContent();
            form.Add(new StringContent($"{token}"), "\"access_token\"");
            form.Add(new StringContent($"{body}"), "\"body\"");
            //form.Add(new StringContent($"https://aruzegaming.com/wp-content/uploads/2014/09/char-lucky-sic-bo.png"), "\"chart\"");

            var bytes = File.ReadAllBytes(filepath);
            var byteContent = new ByteArrayContent(bytes, 0, bytes.Length);
            //byteContent.Headers.Remove("Content-Type");
            byteContent.Headers.Add("Content-Type", "image/png");
            form.Add(byteContent, "\"chart\"", "\"chart.PNG\"");

            using (var httpClient = new HttpClient()) {
                var httpResponse = await httpClient.PostAsync($"{CREATE_URL}", form);
                var response = await httpResponse.Content.ReadAsStringAsync();
            }

        }

        protected virtual async Task<string> SendRequest(string url, string postData, string method = "POST")
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 60000;
            request.Method = method;

            var buffer = Encoding.UTF8.GetBytes(postData);

            using (var writer = new BinaryWriter(await request.GetRequestStreamAsync())) {
                writer.Write(buffer);
            }
            WebResponse webResponse;

            try {
                webResponse = await request.GetResponseAsync();

            } catch (WebException ee) {
                //dto.Response = ee.Message; 
                webResponse = ee.Response;
            }

            //var httpResponse = (HttpWebResponse) webResponse;
            var responseStream = webResponse.GetResponseStream();

            string response;
            using (var sr = new StreamReader(responseStream ?? throw new InvalidOperationException())) {
                response = await sr.ReadToEndAsync();
            }

            return response;
        }

    }
}