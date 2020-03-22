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
using System.IO.Compression;
namespace HttpClientSession
{

    public class HttpStreamInfo : IDisposable
    {
        public HttpResponseMessage HttpResponseMessage { get; }

        public RequestParam RequestParam { get; }

        private byte[] ReceiveBytes { get; set; }
        private MemoryStream Memory { get; set; }
        public HttpStreamInfo(HttpResponseMessage httpResponseMessage, RequestParam request)
        {
            HttpResponseMessage = httpResponseMessage;
            RequestParam = request;
        }

        /// <summary>
        /// 网络流转内存流
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask CopyToAsync( CancellationToken cancellationToken = default)
        {
            //if (!IsGzip)
            //{
                await HttpResponseMessage.Content.CopyToAsync(Memory);
            //}
            //else
            //{
            //    var steam = new GZipStream(await HttpResponseMessage.Content.ReadAsStreamAsync(), CompressionMode.Decompress);
            //    await HttpResponseMessageExtension.CopyToAsync(steam, Memory, cancellationToken);
            //}
            Memory.Position = 0;
            await ReadAsByteAsync(cancellationToken);
            Memory.Dispose();
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask SaveContentAsync(String path, CancellationToken cancellationToken = default)
        {

            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                //await HttpResponseMessageExtension.CopyToAsync(Memory,fs, cancellationToken);
                await fs.WriteAsync(ReceiveBytes, 0, ReceiveBytes.Length);
            }
        }



        /// <summary>
        /// 获取文本
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<string> ReadAsStringAsync(Encoding encoding = null, CancellationToken cancellationToken = default)
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
        public async ValueTask<byte[]> ReadAsByteAsync(CancellationToken cancellationToken = default)
        {
            if (ReceiveBytes == null)
            {
                ReceiveBytes = await HttpResponseMessageExtension.CopyToByteAsync(Memory, cancellationToken);
            }
            return ReceiveBytes;



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
            var b = HttpResponseMessage.Headers.TryGetValues("charSet", out ch);
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

        public void Dispose()
        {
            HttpResponseMessage.Dispose();
            Memory.Dispose();
        }
    }
}
