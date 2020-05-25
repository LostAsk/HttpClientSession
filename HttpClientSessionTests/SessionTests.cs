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

                Url = "http://image.baidu.com/search/detail?z=0&word=%E6%91%84%E5%BD%B1%E5%B8%88%E8%8A%A6%E7%82%B3%E8%87%A3&hs=0&pn=4&spn=0&di=0&pi=1096592873828195119&tn=baiduimagedetail&is=0%2C0&ie=utf-8&oe=utf-8&cs=4144687355%2C1909913594&os=1391503792%2C725646909&simid=&adpicid=0&lpn=0&fm=&sme=&cg=&bdtype=-1&oriquery=&objurl=http%3A%2F%2Ft8.baidu.com%2Fit%2Fu%3D2247852322%2C986532796%26fm%3D79%26app%3D86%26f%3DJPEG%3Fw%3D1280%26h%3D853&fromurl=&gsm=50000000005&catename=pcindexhot&islist=&querylist=",
            };
            using (var s = new Session()) {
                using (var r =await s.SendAsync(p)) {
                    await r.SaveContentAsync(@"C:\Users\Administrator\Desktop\ll.jpg");
                    var tt = 1;
                }
            
            }


            using (var h = new HttpClient()) {
                using (var r = await h.GetAsync("http://image.baidu.com/search/detail?z=0&word=%E6%91%84%E5%BD%B1%E5%B8%88%E8%8A%A6%E7%82%B3%E8%87%A3&hs=0&pn=4&spn=0&di=0&pi=1096592873828195119&tn=baiduimagedetail&is=0%2C0&ie=utf-8&oe=utf-8&cs=4144687355%2C1909913594&os=1391503792%2C725646909&simid=&adpicid=0&lpn=0&fm=&sme=&cg=&bdtype=-1&oriquery=&objurl=http%3A%2F%2Ft8.baidu.com%2Fit%2Fu%3D2247852322%2C986532796%26fm%3D79%26app%3D86%26f%3DJPEG%3Fw%3D1280%26h%3D853&fromurl=&gsm=50000000005&catename=pcindexhot&islist=&querylist=")) {
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
                using (res = await s.SendAsync(p)) {
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
                using (res = await s.SendAsync(p))
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
                using (res = await s.SendAsync(p))
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
                using (var res = await s.SendAsync(p))
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
                using (var res = await s.SendAsync(p))
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