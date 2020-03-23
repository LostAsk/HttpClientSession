using Microsoft.VisualStudio.TestTools.UnitTesting;
using HttpClientSession;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;

namespace HttpClientSession.Tests
{
    [TestClass()]
    public class SessionTests
    {
        [TestMethod()]
        public async Task gbTest() {
            var p = new RequestParam
            {

                Url = "http://www.gds.org.cn/CheckCodeImg.aspx",
            };
            using (var s = new Session()) {
                //s.HttpClientHandler.AllowAutoRedirect = true;
                //s.HttpClientHandler.Proxy = null;
                ////s.HttpClientHandler.AutomaticDecompression = System.Net.DecompressionMethods.None;
                //s.HttpClientHandler.AutomaticDecompression = System.Net.DecompressionMethods.Deflate;
                //s.HeadersUpdate(new Dictionary<string, string>()
                //{
                //    ["Cache-Control"] = "max-age=0",
                //    ["Proxy-Connection"] = "keep-alive",
                //    ["Accept-Encoding"] = "gzip, deflate",
                //    ["Host"] = "www.gds.org.cn",
                //    ["Upgrade-Insecure-Requests"] = "1",
                //    ["Cookie"]= "app_id=0; COLLCK=2652728511; ASP.NET_SessionId=oedbona1bzph5w5mnk0jllhx",
                //}) ;
                using (var r =await s.Send(p)) {
                    await r.SaveContentAsync(@"C:\Users\Administrator\Desktop\ll.jpg");
                    var tt = 1;
                }
            
            }


            using (var h = new HttpClient()) {
                using (var r = await h.GetAsync("http://www.gds.org.cn/CheckCodeImg.aspx")) {
                    using (FileStream fs = new FileStream(@"C:\Users\Administrator\Desktop\ll222.jpg", FileMode.Create))
                    {
                        using (var me = new MemoryStream()) {
                            await r.Content.CopyToAsync(me);
                            me.Position = 0;
                            await me.CopyToAsync(fs);
                        }

                            
                        //await fs.WriteAsync(ReceiveBytes, 0, ReceiveBytes.Length);
                        fs.Flush();
                    }

                }
            
            }
        }



        [TestMethod()]
        public async Task PostFormUrlencodedTest()
        {
            var p = new RequestParam
            {

                Url = "http://httpbin.org/post",
                Headers = new Dictionary<string, string>() {
                    ["user-agent"] = "my-app/0.0.1",
                    ["Cookie"] = "a=1;b=2;c=3;d=4"
                },
                HttpMethod = HttpMethod.Post,
                PostData = new Dictionary<string, object>() { ["a"] = "b", ["c"] = "d" },

            };
            HttpStreamInfo res;
            using (var s = new Session()) {
                using (res = await s.Send(p)) {
                    var r=await res.ReadAsStringAsync();
                    Console.WriteLine(r);
                    var l = 1;
                } ;
                var tt = 1;
            }
            var t = 1;
            
        }

        [TestMethod()]
        public async Task PostJsonTest()
        {
            var p = new RequestParam
            {

                Url = "http://httpbin.org/post",
                Headers = new Dictionary<string, string>()
                {
                    ["user-agent"] = "my-app/0.0.1",
                    ["Cookie"] = "a=1;b=2;c=3;d=4"
                },
                HttpMethod = HttpMethod.Post,
                Json = new Newtonsoft.Json.Linq.JObject { ["a"] = "b", ["c"] = new Newtonsoft.Json.Linq.JObject { ["n"] = "mm" } },

            };
            HttpStreamInfo res;
            using (var s = new Session())
            {
                using (res = await s.Send(p))
                {
                    var r = await res.ReadAsStringAsync();
                    Console.WriteLine(r);
                    var l = 1;
                };
                var tt = 1;
            }
            var t = 1;

        }


        [TestMethod()]
        public async Task PostMultipartTest()
        {
            var f = await File.ReadAllBytesAsync(@"C:\Users\Administrator\Desktop\aa.txt");
            
            var p = new RequestParam
            {

                Url = "http://httpbin.org/post",
                Headers = new Dictionary<string, string>()
                {
                    ["user-agent"] = "my-app/0.0.1",
                    ["Cookie"] = "a=1;b=2;c=3;d=4"
                },
                HttpMethod = HttpMethod.Post,
                PostData=new Dictionary<string, object>() { ["a"]="b",["c"]="d"},
                Files=new List<UploadFile>(new UploadFile[] { new UploadFile { Stream=f,FileName="nn", FieldName="filedname"} })

            };
            HttpStreamInfo res;
            using (var s = new Session())
            {
                using (res = await s.Send(p))
                {
                    var r = await res.ReadAsStringAsync();
                    Console.WriteLine(r);
                    var l = 1;
                };
                var tt = 1;
            }
            var t = 1;

        }


        [TestMethod()]
        public async Task GetTest() {
            var p = new RequestParam
            {

                Url = "http://httpbin.org/get",
                Params=new Dictionary<string, object>() { ["aa"]="bbb",["c"]="dd",},

            };
            using (var s = new Session())
            {
                using (var res = await s.Send(p))
                {
                    var r = await res.ReadAsStringAsync();
                    Console.WriteLine(r);
                    
                    var l = 1;
                };
                var tt = 1;
            }

        }

        [TestMethod()]
        public async Task MerageCookieTest() {
            var c = new Dictionary<string, string>()
            {
                ["a"] = "aaa",
                ["c"] = "ccc"

            };
           
            var p = new RequestParam
            {

                Url = "http://httpbin.org/cookies",
                ///使用临时COOKie，合并本地cookie
                UserCookie = new Dictionary<string, string>() { ["a"] = "ss", ["b"] = "cc" },

                Headers = new Dictionary<string, string>()
                {
                    ["user-agent"] = "my-app/0.0.1",
                    ///使用临时cookie,合并 上面的cookie
                    ["Cookie"] = "a=1;b=2;c=3;d=4"

                },
                HttpMethod = HttpMethod.Get,
            };

            using (var s = new Session())
            {
                ///更新本地cookie
                s.CookieContainerUpdate(c);
                using (var res = await s.Send(p))
                {
                    var r = await res.ReadAsStringAsync();
                    Console.WriteLine(r);
                    var l = 1;
                };
                var tt = 1;
            }


        }


    }
}