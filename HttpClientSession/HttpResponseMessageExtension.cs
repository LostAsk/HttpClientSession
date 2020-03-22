using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Net.Http;
namespace HttpClientSession
{
   public static class HttpResponseMessageExtension
   {
        public static async ValueTask CopyToAsync(this Stream source, Stream dest, CancellationToken cancellationToken)
        {
            var buffer = BytesPool.Rent(256 * 256 * 256);
            var totalBytes = 0L;
            int bytesCopied;
            try
            {
                do
                {
                    bytesCopied =await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                    await dest.WriteAsync(buffer, 0, bytesCopied, cancellationToken);
                    totalBytes += bytesCopied;
                } while (bytesCopied > 0);
            }
            finally
            {
                BytesPool.Return(buffer);
            }
        }

        public static async ValueTask<byte[]> CopyToByteAsync(this Stream source, CancellationToken cancellationToken)
        {

            var buffer = new byte[source.Length];

            await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
            return buffer;
        }
    }
}
