using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
namespace HttpClientSession
{
    internal static class BytesPool
    {
        public static readonly ConcurrentDictionary<int, ConcurrentBag<byte[]>> _BytesPool = new ConcurrentDictionary<int, ConcurrentBag<byte[]>>();

        public static byte[] Rent(int size) => _BytesPool.GetOrAdd(size, new ConcurrentBag<byte[]>()).TryTake(out var bytes) ? bytes : new byte[size];

        public static void Return(byte[] bytes) => _BytesPool.GetOrAdd(bytes.Length, new ConcurrentBag<byte[]>()).Add(bytes);
    }



}
