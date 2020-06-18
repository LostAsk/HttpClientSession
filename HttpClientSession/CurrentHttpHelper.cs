using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
namespace HttpClientSession
{
    public static class CurrentHttpHelper
    {


        /// <summary>
        /// 尝试请求
        /// </summary>
        /// <param name="s"></param>
        /// <param name="x"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static async Task<HttpStreamInfo> TryRequests(this Session s, RequestParam x, int num = 3,int delay=2, CancellationToken cancellationToken = default)
        {
            Func<HttpStreamInfo, ValueTask<bool>> check = (r) => new ValueTask<bool>(r.HttpResponseMessage.IsSuccessStatusCode == true);
            return await TryRequests(s, x, check, num, delay, cancellationToken);
        }
        /// <summary>
        /// 尝试重连
        /// </summary>
        /// <param name="s">Session</param>
        /// <param name="x">RequestsParam</param>
        /// <param name="CheckFunc">检查HTML委托</param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static async Task<HttpStreamInfo> TryRequests(this Session s, RequestParam x, Func<HttpStreamInfo,ValueTask<bool>> CheckFunc, int num = 3,int delay=2,CancellationToken cancellationToken=default)
        {
            Exception e = null;
            for (var i = 0; i < num; i++)
            {
                try
                {
                    using (var r =await s.SendAsync(x, cancellationToken))
                    {
                        if (await CheckFunc(r))
                        {
                            return r;
                        }
                    }
                }
                catch (Exception ex)
                {
                    e = ex;
                   await Task.Delay(delay*1000);
                }
            }
            throw e;
        }


        public static async Task<HttpStreamInfo[]> GetHttpResponseAsync(this Session sess,IEnumerable<RequestParam> items, Func<HttpStreamInfo,ValueTask<bool>> CheckFunc, int current_num = 10, int delay = 1, int num =3)
        {
            using (var semaphore = new SemaphoreSlim(current_num))
            {
                var rnd = new Random();
                var s = sess;
                var tasks = items.Select(async (item) =>
                {
                    await semaphore.WaitAsync();
                   
                    try
                    {
                        Exception e = null;
                        for (var i = 0; i < num; i++)
                        {
                            try
                            {
                                using (var r = await s.SendAsync(item))
                                {
                                    if (await CheckFunc(r))
                                    {
                                        return r;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                e = ex;
                                await Task.Delay(rnd.Next(0, (delay * 100)));
                            }

                        }
                        throw e;
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }
                ).ToArray();
                return await Task.WhenAll(tasks);
            };


        }
    }
}
