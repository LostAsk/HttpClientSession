using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Text;
namespace HttpClientSession
{
    public class RequestParam
    {
        public string Url { get; set; }


        public HttpMethod HttpMethod { get; set; } = HttpMethod.Get;

        public Encoding Encoding { get; set; } = Encoding.UTF8;

        public Dictionary<string, string> Headers { get; set; }
        public IEnumerable<KeyValuePair<string,string>> Params { get; set; }

        public HttpContent PostData { get; set; }

        public Dictionary<String, String> UserCookie { get; set; }

        public Action<HttpRequestMessage> UserHander { get; set; }

        public HttpCompletionOption HttpCompletionOption { get; set; } = HttpCompletionOption.ResponseContentRead;
        public Object OtherInfo { get; set; }
    }
}
