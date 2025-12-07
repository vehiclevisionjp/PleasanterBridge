using PleasanterAPIBridge.src.PleasanterAPIBridge.Contracts;
using System;
using System.Net;
using System.Net.Http;

namespace PleasanterAPIBridge.src.PleasanterAPIBridge.Exception
{
    internal class PleasanterApiException : System.Exception
    {
        public PleasanterApiResponse ResponseInfo { get; }

        public HttpStatusCode StatusCode => (HttpStatusCode)ResponseInfo.StatusCode;

        // コンストラクタ
        public PleasanterApiException(
            string message,
            PleasanterApiResponse responseInfo,
            System.Exception? innerException = null)
            : base(message, innerException)
        {
            ResponseInfo = responseInfo;
        }
        public override string ToString()
        {
            return $"Pleasanter API Error ({StatusCode}): {Message}\n" +
                   $"URL: {ResponseInfo.Url}\n" +
                   $"Response Message: {ResponseInfo.Message}\n" +
                   $"Sent Data: {ResponseInfo.SendData}\n" +
                   $"{base.ToString()}";
        }
    }
}
