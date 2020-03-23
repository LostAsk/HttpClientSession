using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace HttpClientSession
{


    public class Session:IDisposable
    {
        public HttpClientHandler HttpClientHandler { get; }

        private Dictionary<string, string> SessionCookieContainer;
        public HttpClient HttpClient { get; }

        public HttpRequestHeaders HttpRequestHeaders { get => HttpClient.DefaultRequestHeaders; }


        public Session()
        {
            HttpClientHandler = new HttpClientHandler();
            //{ UseCookies = false,CookieContainer=new CookieContainer() };
            HttpClient = new HttpClient(HttpClientHandler) { Timeout = TimeSpan.FromMinutes(60), };
            SetHttpClientHandler();
            SetSessionCookieFromHttpClientHandler();
            DefaultReqInit();
        }

        public Session(HttpClientHandler httpClientHandler) {
            HttpClientHandler = httpClientHandler;
            //HttpClientHandler.UseCookies = false;
            HttpClient = new HttpClient(HttpClientHandler);
            DefaultReqInit();
            SetSessionCookieFromHttpClientHandler();

        }

        public Session(HttpClient httpClient, HttpClientHandler httpClientHandler) {
            HttpClient = httpClient;
            HttpClientHandler = httpClientHandler;
            //HttpClientHandler.UseCookies = false;
            SetSessionCookieFromHttpClientHandler();
        }

        /// <summary>
        /// 默认请求初始化
        /// </summary>
        /// <param name="Req"></param>
        private void DefaultReqInit()
        {
            //HttpRequestHeaders.Add("Accept", "*/*");
            HttpRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.149 Safari/537.36");
        }

        private void SetSessionCookieFromHttpClientHandler() {
            SessionCookieContainer =  CookieHelper.GetAllCookies(HttpClientHandler.CookieContainer);
        }


        /// <summary>
        /// 初始化
        /// </summary>
        private void SetHttpClientHandler()
        {
            RequstsHelper.SetServerCertificateCustomValidationCallback(HttpClientHandler);
        }
        /// <summary>
        /// 更新httpclient头设置
        /// </summary>
        /// <param name="headers"></param>
        public void HeadersUpdate(Dictionary<string, string> headers)
        {
            RequstsHelper.UpdateRequestHeader(HttpRequestHeaders, headers);
        }
        /// <summary>
        /// 更新sesion cookiejar
        /// </summary>
        public void CookieContainerUpdate(Dictionary<string, string> cookies) {
            CookieHelper.MergeCookie(SessionCookieContainer, cookies);
        }

        public string GetCookieString() {
            return CookieHelper.GetCookieString(SessionCookieContainer);
        }

        public async Task<HttpStreamInfo> Send(RequestParam requestParam) {
            return await Send(requestParam, new CancellationToken());
        }

        public async Task<HttpStreamInfo> Send(RequestParam requestParam,CancellationToken cancellationToken)
        {
            using (var httpmessage = GetHttpRequestMessage(requestParam)) {
        
                using (var res = await HttpClient.SendAsync(httpmessage, requestParam.HttpCompletionOption, cancellationToken)) 
                {
                    var rescookie = CookieHelper.GetCookieFromResponseHeader(res.Headers);
                    CookieHelper.MergeCookie(SessionCookieContainer, rescookie);
                    var streaminfo = new HttpStreamInfo(res,requestParam);
                    await streaminfo.CopyToAsync(cancellationToken);

                    return streaminfo;
                }
            };

        }

        /// <summary>
        /// 生成HttpRequestMessage
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        private HttpRequestMessage GetHttpRequestMessage(RequestParam requestParam) {
            var url = RequstsHelper.PrepareUrl(requestParam.Url, requestParam.Params, requestParam.Encoding);
            var httpmessage = new HttpRequestMessage(requestParam.HttpMethod, url);
            RequstsHelper.UpdateRequestHeader(httpmessage.Headers, requestParam.Headers);
            CreateCookieHeader(httpmessage.Headers, requestParam.UserCookie);
            httpmessage.Content = CreateContent(requestParam);
            requestParam.UserHander?.Invoke(httpmessage);
            return httpmessage;
        }

        /// <summary>
        /// 创建HttpContent对象
        /// </summary>
        /// <param name="requestParam"></param>
        /// <returns></returns>
        private static HttpContent CreateContent(RequestParam requestParam) {
            HttpContent http=null;
            if (requestParam.HttpMethod == HttpMethod.Get || requestParam.HttpMethod == HttpMethod.Head) {
                return http;
            }
            if (requestParam.SendData != null)
            {
                http = new ByteArrayContent(requestParam.SendData);
                http.Headers.ContentType = requestParam.MediaTypeHeaderValue;
                http.Headers.ContentType.CharSet = requestParam.Encoding.WebName;
                return http;
            }
            if (requestParam.Json != null) {
                http = new StringContent(requestParam.Json.ToString(), requestParam.Encoding);
                http.Headers.ContentType = ContentType.CreateJson();
                http.Headers.ContentType.CharSet = requestParam.Encoding.WebName;
                return http;
            }
            if (requestParam.PostData != null && requestParam.Files == null) {
                http = new FormUrlEncodedContent(RequstsHelper.DicToEnumerableKeyPair(requestParam.PostData, requestParam.Encoding));
                http.Headers.ContentType = ContentType.CreateFormUrlencoded();
                http.Headers.ContentType.CharSet = requestParam.Encoding.WebName;
                return http;
            }
            if (requestParam.PostData != null && requestParam.Files != null) {
                var type = string.Empty;
                http = new ByteArrayContent(RequstsHelper.DicToMsMultiPartFormDataBytes(requestParam.PostData, out type, requestParam.Files, requestParam.Encoding));
                http.Headers.Add("Content-Type", type);
                //http.Headers.ContentType.CharSet = requestParam.Encoding.WebName;
                return http;
            }
            return http;

        }


        /// <summary>
        /// 合并头信息
        /// </summary>
        /// <param name="tmpheaders"></param>
        /// <returns></returns>
        private Dictionary<string, string> MerrageCookieFromHeader(HttpRequestHeaders tmpheaders) {
            if (tmpheaders.Contains("cookie")) { return CookieHelper.CookieDicFromCookieStr(tmpheaders.GetValues("cookie").ToArray()[0]); }
            if (!tmpheaders.Contains("cookie") && HttpRequestHeaders.Contains("cookie")) { return CookieHelper.CookieDicFromCookieStr(HttpRequestHeaders.GetValues("cookie").ToArray()[0]); }
            return null;
        }
        /// <summary>
        /// 合并cookie
        /// </summary>
        /// <param name="cookies"></param>
        /// <returns></returns>
        private Dictionary<string, string> MerrageCookieFromCookieJar(Dictionary<string, string> cookies) {
            var tmp = SessionCookieContainer;
            if (cookies == null) return tmp;
            else {
                foreach (var c in tmp) {
                    if (!cookies.ContainsKey(c.Key)) { cookies[c.Key] = c.Value; }
                }
                return cookies;
            }
        }
        /// <summary>
        /// 最终合并cookie
        /// </summary>
        /// <param name="tmpheaders"></param>
        /// <param name="tmpcookies"></param>
        /// <returns></returns>
        private string CreateCookieHeader(HttpRequestHeaders tmpheaders, Dictionary<string, string> tmpcookies) {

            var tmpheadersdic = MerrageCookieFromHeader(tmpheaders);
            var tmpcookedic = MerrageCookieFromCookieJar(tmpcookies);
            var value = string.Empty;
            tmpheaders.Remove("cookie");
            if (tmpheadersdic == null)
            {
                value = string.Join(";", tmpcookedic.Select(x => x.Key + "=" + x.Value));
            }
            else {
                foreach (var c in tmpcookedic) {
                    tmpheadersdic[c.Key] = c.Value;
                }
                value = string.Join(";", tmpheadersdic.Select(x => x.Key + "=" + x.Value));
                
            }
            if(!string.IsNullOrWhiteSpace("cookie")) tmpheaders.Add("cookie", value);
            return value;
        }

        public void Dispose() {
            HttpClient.Dispose();
            HttpClientHandler.Dispose();
        }
    }


}
