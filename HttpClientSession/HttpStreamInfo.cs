using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Linq;
namespace HttpClientSession
{

    public class HttpStreamInfo : IDisposable
    {
       public HttpResponseMessage HttpResponseMessage { get; }

        private MemoryStream Memory { get; set; } = new MemoryStream();
        public HttpStreamInfo(HttpResponseMessage httpResponseMessage) {
            HttpResponseMessage = httpResponseMessage;

       }

        /// <summary>
        /// 网络流转内存流
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task CopyToAsync(CancellationToken cancellationToken = default) {
            await HttpResponseMessage.Content.CopyToAsync(Memory);
            Memory.Position = 0;
        }
        
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SaveContentAsync(String path, CancellationToken cancellationToken=default)
        {
            
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                await HttpResponseMessageExtension.CopyToAsync(Memory,fs, cancellationToken);

            }
        }



        /// <summary>
        /// 获取文本
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> ReadAsStringAsync(Encoding encoding=null, CancellationToken cancellationToken=default)
        {
            var ResponseByte = await ReadAsByteAsync(cancellationToken);
            if (encoding == null)
            {
                var encode = GetEncoding(ResponseByte);
                return encode.GetString(ResponseByte);
            }
            else
            {
                return encoding.GetString(ResponseByte);
            }
        }

        /// <summary>
        /// 转steam
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<byte[]> ReadAsByteAsync(CancellationToken cancellationToken = default) {
            return await HttpResponseMessageExtension.CopyToByteAsync(Memory, cancellationToken);

        }

        /// <summary>  
        /// 获取编码 
        /// </summary>  
        /// <params name="HttpWebResponse">HttpWebResponse对象</params>
        /// <params name="httpResponseByte">HttpWebResponse对象的实体流</params>
        /// <returns>Encoding</returns> 
        private Encoding GetEncoding(byte[] httpResponseByte = null)
        {
            var CharacterSet = string.Empty;
            IEnumerable<string> ch;
            var b=HttpResponseMessage.Headers.TryGetValues("charSet", out ch);
            if (b) { CharacterSet = ch.ToArray()[0]; }
            Encoding encoding = null;
            if (httpResponseByte != null)
            {
                Match meta = Regex.Match(Encoding.Default.GetString(httpResponseByte), "<meta[^<]*charset=([^<]*)[\"']", RegexOptions.IgnoreCase);
                string c = string.Empty;
                if (meta != null && meta.Groups.Count > 0)
                {
                    c = meta.Groups[1].Value.ToLower().Trim();
                }
                if (c.Length > 2)
                {
                    try
                    {
                        encoding = Encoding.GetEncoding(c.Replace("\"", string.Empty).Replace("'", "").Replace(";", "").Replace("iso-8859-1", "gbk").Trim());
                    }
                    catch
                    {
                        if (string.IsNullOrEmpty(CharacterSet))
                        {
                            encoding = Encoding.UTF8;
                        }
                        else
                        {
                            encoding = Encoding.GetEncoding(CharacterSet);
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(CharacterSet))
                    {
                        encoding = Encoding.UTF8;
                    }
                    else
                    {
                        encoding = Encoding.GetEncoding(CharacterSet);
                    }
                }



            }
            return encoding;



        }

        public void Dispose() {
            HttpResponseMessage.Dispose();
            Memory.Dispose();
        }
    }
}
