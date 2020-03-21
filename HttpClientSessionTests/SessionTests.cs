using Microsoft.VisualStudio.TestTools.UnitTesting;
using HttpClientSession;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
namespace HttpClientSession.Tests
{
    [TestClass()]
    public class SessionTests
    {
        [TestMethod()]
        public async Task SendTest()
        {
            var c = new Dictionary<string, string>() {
                ["a"]="aaa",
                ["c"]="ccc"
            
            };
            var p = new RequestParam
            {
                Url = "http://httpbin.org/cookies",
                UserCookie = new Dictionary<string, string>() { ["a"] = "ss",["b"]="cc" },
                Headers=new Dictionary<string, string>() { 
                    ["user-agent"]= "my-app/0.0.1",
                    ["Cookie"]="a=1;b=2;c=3;d=4"
                
                }
            };
            
            using (var s = new Session()) {
                s.CookieContainerUpdate(c);
                var res = await s.Send(p);
                await res.SaveContentAsync(@"C:\Users\Administrator\Desktop\mm.html");
                var tt = 1;

            }
            
        }
    }
}