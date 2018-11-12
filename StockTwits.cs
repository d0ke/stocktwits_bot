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
        //private const string CREATE_URL = "https://crmapi2.thesafetransaction.com/api/v1/common/content";

        public StockTwitsClient()
        {
            
        }

        public async Task<string> Authorize(string clientid, string clientsecret, string code, string redirectUri)
        {
            var postData =
                $"client_id={clientid}&client_secret={clientsecret}&code={code}&grant_type=authorization_code&redirect_uri={redirectUri}";

            return await SendRequest(AUTH_URL, postData);
        }


        public async Task CreateMessage(string token, string body)
        {
            await SendRequest($"{CREATE_URL}?access_token={token}", $"body={body}&chart=https://charts.stocktwits.com/production/original_144406812.png");
        }


        //public async Task CreateMessageWithChart(string body, string filepath)
        //{
        //    string boundary = CreateFormDataBoundary();
        //    var contentType = "multipart/form-data; boundary=" + boundary;
        //    var parameters = new Dictionary<string, string> {
        //        {"access_token", TOKEN},
        //        {"body", body},
        //        {"chart", "https://charts.stocktwits.com/production/original_144406812.png"}
        //    };
        //    var content = WriteMultipartFormData(parameters, boundary);

        //    await SendRequest($"{CREATE_URL}?access_token={TOKEN}", content);
        //}




        public async Task CreateMessageWithChartMultipart(string body, string filepath)
        {
            var bytes = File.ReadAllBytes(filepath);
            var form = new MultipartFormDataContent();

            var c1 = new StringContent($"{TOKEN}");
            //c1.Headers.Remove("Content-Type");

            var c2 = new StringContent($"{body}");
            //c2.Headers.Remove("Content-Type");

            var c3 = new StringContent($"https://aruzegaming.com/wp-content/uploads/2014/09/char-lucky-sic-bo.png");
            //c3.Headers.Remove("Content-Type");

            form.Add(c1, "access_token");
            form.Add(c2, "body");
            //form.Add(c3, "\"chart\"");

            var byteContent = new ByteArrayContent(bytes, 0, bytes.Length);
            //byteContent.Headers.Remove("Content-Type");
            //byteContent.Headers.Add("Content-Type", "image/png");
            form.Add(byteContent, "chart");

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


        private const string FormDataTemplate = "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n";
        
        private static string CreateFormDataBoundary()
        {
            return "---------------------------" + DateTime.Now.Ticks.ToString("x");
        }

        private static string WriteMultipartFormData(Dictionary<string, string> dictionary, string mimeBoundary)
        {
            if (dictionary == null || dictionary.Count == 0)
                throw new ArgumentNullException(nameof(dictionary));

            if (mimeBoundary == null)
                throw new ArgumentNullException(nameof(mimeBoundary));

            if (mimeBoundary.Length == 0)
                throw new ArgumentException("MIME boundary may not be empty.", nameof(mimeBoundary));

            var sb = new StringBuilder();

            foreach (string key in dictionary.Keys) {
                string item = String.Format(FormDataTemplate, mimeBoundary, key, dictionary[key]);
                sb.Append(item);
                //byte[] itemBytes = Encoding.UTF8.GetBytes(item);
                //stream.Write(itemBytes, 0, itemBytes.Length);
            }

            var postData = sb.ToString();
            return postData;
        }
    }
}