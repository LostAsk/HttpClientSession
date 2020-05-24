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

                Url = "https://image.baidu.com/search/detail?ct=503316480&z=0&ipn=false&word=%E5%90%B4%E5%BD%A6%E7%A5%96&step_word=&hs=0&pn=0&spn=0&di=167310&pi=0&rn=1&tn=baiduimagedetail&is=0%2C0&istype=2&ie=utf-8&oe=utf-8&in=&cl=2&lm=-1&st=-1&cs=4254164563%2C3923686114&os=3469395836%2C1843962168&simid=3299881063%2C113253717&adpicid=0&lpn=0&ln=3330&fr=&fmq=1590304672285_R&fm=index&ic=0&s=undefined&hd=undefined&latest=undefined&copyright=undefined&se=&sme=&tab=0&width=&height=&face=undefined&ist=&jit=&cg=star&bdtype=0&oriquery=&objurl=http%3A%2F%2F5b0988e595225.cdn.sohucs.com%2Fq_mini%2Cc_zoom%2Cw_640%2Fimages%2F20170728%2F5843abd8cdb74745a2fe2349879cb055.jpeg&fromurl=ippr_z2C%24qAzdH3FAzdH3F4_z%26e3Bf5i7_z%26e3Bv54AzdH3FwAzdH3F8macld9mb_dc0nnb%3F_u%3D4-w6ptvsj_8l_ujj1f_dd&gsm=1&rpstart=0&rpnum=0&islist=&querylist=&force=undefined",
            };
            using (var s = new Session()) {
                using (var r =await s.SendAsync(p)) {
                    await r.SaveContentAsync(@"C:\Users\Administrator\Desktop\ll.jpg");
                    var tt = 1;
                }
            
            }


            using (var h = new HttpClient()) {
                using (var r = await h.GetAsync("https://image.baidu.com/search/detail?ct=503316480&z=0&ipn=false&word=%E5%90%B4%E5%BD%A6%E7%A5%96&step_word=&hs=0&pn=0&spn=0&di=167310&pi=0&rn=1&tn=baiduimagedetail&is=0%2C0&istype=2&ie=utf-8&oe=utf-8&in=&cl=2&lm=-1&st=-1&cs=4254164563%2C3923686114&os=3469395836%2C1843962168&simid=3299881063%2C113253717&adpicid=0&lpn=0&ln=3330&fr=&fmq=1590304672285_R&fm=index&ic=0&s=undefined&hd=undefined&latest=undefined&copyright=undefined&se=&sme=&tab=0&width=&height=&face=undefined&ist=&jit=&cg=star&bdtype=0&oriquery=&objurl=http%3A%2F%2F5b0988e595225.cdn.sohucs.com%2Fq_mini%2Cc_zoom%2Cw_640%2Fimages%2F20170728%2F5843abd8cdb74745a2fe2349879cb055.jpeg&fromurl=ippr_z2C%24qAzdH3FAzdH3F4_z%26e3Bf5i7_z%26e3Bv54AzdH3FwAzdH3F8macld9mb_dc0nnb%3F_u%3D4-w6ptvsj_8l_ujj1f_dd&gsm=1&rpstart=0&rpnum=0&islist=&querylist=&force=undefined")) {
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