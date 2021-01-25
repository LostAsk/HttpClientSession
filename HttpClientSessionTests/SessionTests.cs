using Microsoft.VisualStudio.TestTools.UnitTesting;
using HttpClientSession;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;

namespace HttpClientSession.Tests
{

    public class ImageDistinguishService 
    {
        private Session Session { get; } = new Session();

        private const string Url = "https://aip.baidubce.com/rest/2.0/ocr/v1/general_basic";

        private const string AccessTokenUrl = "https://aip.baidubce.com/oauth/2.0/token";
        
        private string Token { get; set; }

        public async Task<string> GetStringByImageAsync(string imgbase64string, string url)
        {
            var tmpurl = string.IsNullOrEmpty(url) ? string.Empty : url.Contains("http://") ? url : "http://" + url;
            var PostData = new Dictionary<string, object>
            {
                ["image"] = imgbase64string,
                ["url"] = tmpurl,
                //["language_type"] = "CHN_ENG",
                //["detect_direction"] = "true",
                //["probability"] = "true",
            };
            var par = new RequestParam
            {
                Url = Url,
                HttpMethod = HttpMethod.Post,
                Params = new Dictionary<string, object>()
                {
                    ["access_token"] = Token,

                },

                MediaTypeHeaderValue = ContentType.CreateFormUrlencoded(),

                //Encoding=Encoding.Default
            };
            if (string.IsNullOrEmpty(imgbase64string) && !string.IsNullOrEmpty(tmpurl))
            {
                PostData.Remove("image");
            }
            else if (!string.IsNullOrEmpty(imgbase64string) && string.IsNullOrEmpty(tmpurl))
            {
                PostData.Remove("url");
            }
            else
            {
                PostData.Remove("url");
            }
            var data = string.Join("&", RequstsHelper.DicToEnumerableKeyPairEncode(PostData).Select(x => x.Key + "=" + x.Value));
            par.SendData = Encoding.UTF8.GetBytes(data);
            using (var res = await Session.SendAsync(par))
            {
                var html = await res.ReadAsStringAsync();
                return html;
                //return JsonConvert.DeserializeObject<JObject>(html);
            }

            //return Task.FromResult("222");
        }

        public async Task<JObject> GetAccessTokenAsync(string key ,string value)
        {
            var par = new RequestParam
            {
                Url = AccessTokenUrl,
                HttpMethod = HttpMethod.Post,
                Params = new Dictionary<string, object>()
                {
                    ["grant_type"] = "client_credentials",
                    ["client_id"] = key,
                    ["client_secret"] = value,

                },
            };
            using (var res = await Session.SendAsync(par))
            {
                var html = await res.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<JObject>(html);
                if (json["error"] != null)
                {
                    throw new Exception("授权失败,请检查APPKey和Secret Key");
                }
                Token = json.Value<string>("access_token");
                //ConfigOption.BaiduConfig.Token = token;
                return null;
            }
        }
    }
    [TestClass()]
    public class SessionTests
    {
        [TestMethod()]
        public async Task gbTest() {
            var p = new RequestParam
            {

                Url = "http://www.baidu.com",
            };
            using (var s = new Session()) {
                using (var r =await s.SendAsync(p)) {
                    var html = await r.ReadAsStringAsync();
                    var tt = 1;
                }
            
            }
        }

        [TestMethod()]
        public async Task baiduTest()
        {
           
            var par = @"C:\Users\Administrator\Desktop\ff\zz.png";
            var bs = File.ReadAllBytes(par);
            var base64 = Convert.ToBase64String(bs);
            var key = "xx";
            var value = "ff";
              
            var service = new ImageDistinguishService();
             await service.GetAccessTokenAsync(key,value);
            
           
            var result = await service.GetStringByImageAsync(base64, string.Empty);
            var tt = 1;
        }



        [TestMethod()]
        public async Task PostFormUrlencodedTest()
        {
            var p = new RequestParam
            {

                Url = "https://httpbin.org/post",
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
        public async Task HttpClientPostMultipartTest() {
            using (var client = new HttpClient()) {
                var fs = await File.ReadAllBytesAsync(@"C:\Users\Administrator\Desktop\timg.jpg");
                MultipartFormDataContent mulContent = new MultipartFormDataContent("----");//创建用于可传递文件的容器
                HttpContent fileContent = new ByteArrayContent(fs);
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");//设置媒体类型
                mulContent.Add(fileContent, "file", "timg.jpg");//第二个参数是表单名，第三个是文件名。
              
                mulContent.Add(new StringContent("aaaaa"), "User");
                using (var res = await client.PostAsync("https://localhost:44374/api/account/UpdateImage", mulContent)) {
                    var html = await res.Content.ReadAsStringAsync();
                    var tt = 1;
                }
            }
        }


        [TestMethod()]
        public async Task PostMultipartTest()
        {
            var f = await File.ReadAllBytesAsync(@"C:\Users\Administrator\Desktop\timg.jpg");
            
            var p = new RequestParam
            {

                Url = "https://localhost:44374/api/account/UpdateImage",
                Headers = new Dictionary<string, string>()
                {
                    ["user-agent"] = "my-app/0.0.1",
                    ["Cookie"] = "a=1;b=2;c=3;d=4"
                },
                HttpMethod = HttpMethod.Post,
                PostData=new Dictionary<string, object>() { ["User"] ="zzzzzzzz",["c"]="d"},
                Files=new List<UploadFile>(new UploadFile[] { new UploadFile { Stream=f,FileName="nn.jpg", FieldName="filedname"} })

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