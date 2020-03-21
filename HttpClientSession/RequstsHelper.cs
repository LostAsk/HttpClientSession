using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Dynamic;
namespace HttpClientSession
{
    internal static class RequstsHelper
    {
        /// <summary>
        /// 对参数编成URL的形式
        /// </summary>
        /// <param name="param"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        internal static string EncodeParams(IEnumerable<KeyValuePair<string, string>> param, Encoding encoding)
        {
            var encodpar = string.Empty;
            if (param == null) { return encodpar; }
            var encode = encoding ?? Encoding.UTF8;
            List<String> pars = new List<String>();
            foreach (KeyValuePair<String, string> par in param)
            {
                pars.Add(System.Web.HttpUtility.UrlEncode(par.Key, encode) + "=" + System.Web.HttpUtility.UrlEncode(par.Value, encode));
            }
            encodpar = String.Join("&", pars);
            return encodpar;

        }

        /// <summary>
        /// Url预处理
        /// </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <param name="encoding"></param>
        internal static string PrepareUrl(string url, IEnumerable<KeyValuePair<string, string>> param, Encoding encoding) {
            var Url = string.Empty;
            if (!(url.Contains("http://") || url.Contains("https://")))
            {
                url = "http://" + url;
            }
            if (param == null) {    Url= url; return Url; }
            var uri = new Uri(url);
            if (String.IsNullOrWhiteSpace(uri.Query)) { Url = url + "?" + RequstsHelper.EncodeParams(param, encoding); return Url; }
            else { Url = url + "&" + RequstsHelper.EncodeParams(param, encoding); return Url; }
        }

        /// <summary>
        /// 更新头信息
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="headerdic"></param>
        internal static void UpdateRequestHeader(HttpRequestHeaders headers, Dictionary<string, string> headerdic) {
            if (headerdic == null) return;
            foreach (var kv in headerdic) {
                if (headers.Contains(kv.Key)){
                    headers.Remove(kv.Key);
                }
                headers.Add(kv.Key, kv.Value);
            }
        }

        public static void SetServerCertificateCustomValidationCallback(HttpClientHandler httpClientHandler) {
            httpClientHandler.ServerCertificateCustomValidationCallback = (reqmessage, x509cert, x509chain, sslp) => true;
        }


    }
}
