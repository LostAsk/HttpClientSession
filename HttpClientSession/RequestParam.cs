using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
namespace HttpClientSession
{
    /// <summary>
    /// 请求参数
    /// post（优先顺序发UserSendData,然后json，然后urlencoded,然后MultiPartFormData,）
    /// </summary>
    public class RequestParam
    {
        /// <summary>
        /// url
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 请求方式
        /// </summary>
        public HttpMethod HttpMethod { get; set; } = HttpMethod.Get;
        /// <summary>
        /// 编码
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        /// <summary>
        /// 请求头设置
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }
        /// <summary>
        /// Get参数
        /// </summary>
        public Dictionary<string,object> Params { get; set; }


        /// <summary>
        /// Post参数
        /// </summary>
        public Dictionary<string,object> PostData { get; set; }
        /// <summary>
        /// post传Json参数
        /// </summary>
        public JObject Json { get; set; }
        /// <summary>
        /// 上传文件参数
        /// </summary>
        public List<UploadFile> Files { get; set; }

        /// <summary>
        /// 自定义cookie
        /// </summary>
        public Dictionary<String, String> UserCookie { get; set; }

        /// <summary>
        /// 对HttpRequestMessage额外操作
        /// </summary>
        public Action<HttpRequestMessage> UserHander { get; set; }
        /// <summary>
        /// 读取内容方式
        /// </summary>
        public HttpCompletionOption HttpCompletionOption { get; set; } = HttpCompletionOption.ResponseContentRead;

        /// <summary>
        /// 额外参数
        /// </summary>
        public Object OtherInfo { get; set; }

        /// <summary>
        /// 自定义设置头MediaType （配合UserSendData一起用）
        /// </summary>
        public MediaTypeHeaderValue MediaTypeHeaderValue { get; set; }
      
        /// <summary>
        /// 发送参数
        /// </summary>
        public byte[] SendData { get; set; }

    }
}
