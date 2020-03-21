using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Dynamic;
namespace HttpClientSession
{
    public static class RequstsHelper
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
        internal static string PrepareUrl(string url, IEnumerable<KeyValuePair<string, string>> param, Encoding encoding)
        {
            var Url = string.Empty;
            if (!(url.Contains("http://") || url.Contains("https://")))
            {
                url = "http://" + url;
            }
            if (param == null) { Url = url; return Url; }
            var uri = new Uri(url);
            if (String.IsNullOrWhiteSpace(uri.Query)) { Url = url + "?" + RequstsHelper.EncodeParams(param, encoding); return Url; }
            else { Url = url + "&" + RequstsHelper.EncodeParams(param, encoding); return Url; }
        }

        /// <summary>
        /// 更新头信息
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="headerdic"></param>
        internal static void UpdateRequestHeader(HttpRequestHeaders headers, Dictionary<string, string> headerdic)
        {
            if (headerdic == null) return;
            foreach (var kv in headerdic)
            {
                if (headers.Contains(kv.Key))
                {
                    headers.Remove(kv.Key);
                }
                headers.Add(kv.Key, kv.Value);
            }
        }

        internal static void SetServerCertificateCustomValidationCallback(HttpClientHandler httpClientHandler)
        {
            httpClientHandler.ServerCertificateCustomValidationCallback = (reqmessage, x509cert, x509chain, sslp) => true;
        }


        /// <summary>
        /// 对参数编成IEnumerable<KeyValuePair<string, string>>
        /// </summary>
        /// <param name="param"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, string>> DicToIemableKeyPair(Dictionary<string, Object> param, Encoding encoding)
        {
            var encodpar = string.Empty;
            if (param == null) { throw new Exception("param不能为空"); }
            var encode = encoding ?? Encoding.UTF8;
            List<KeyValuePair<String, string>> pars = new List<KeyValuePair<String, string>>();
            foreach (KeyValuePair<String, dynamic> par in param)
            {
                if (par.Value.GetType().IsValueType || par.Value.GetType() == typeof(String))
                {
                    pars.Add(new KeyValuePair<string, string>(System.Web.HttpUtility.UrlEncode(par.Key, encode), System.Web.HttpUtility.UrlEncode(par.Value, encode)));
                }
                else if (typeof(IEnumerable<string>).IsAssignableFrom(par.Value.GetType()))
                {
                    var tmp = (par.Value as IEnumerable<string>).Select(x =>
                         new KeyValuePair<string, string>(System.Web.HttpUtility.UrlEncode(par.Key, encode), System.Web.HttpUtility.UrlEncode(x, encode))
                    );
                    pars.AddRange(tmp);
                }
                else
                {
                    throw new Exception("目前值只接受值String|IEnumerable<String>的字典");
                }

            }

            return pars;

        }
    }
}
