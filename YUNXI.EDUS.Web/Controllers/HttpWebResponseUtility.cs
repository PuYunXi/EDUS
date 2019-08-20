using System;
using System.Net;

namespace YUNXI.EDUS.Web.Controllers
{
    public class HttpWebResponseUtility
    {
        private static readonly string DefaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36";

        /// <summary>  
        /// 创建GET方式的HTTP请求  
        /// </summary>  
        /// <param name="url">请求的URL</param>  
        /// <param name="timeout">请求的超时时间</param>  
        /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>  
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>  
        /// <returns></returns>  
        public static HttpWebResponse CreateGetHttpResponse(string url, int? timeout, string userAgent, CookieCollection cookies)
        {
            HttpWebResponse httpWebResponse;

            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            var request = WebRequest.Create(url) as HttpWebRequest;

            if (request == null) return null;

            request.Method = "GET";

            request.UserAgent = DefaultUserAgent;

            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }

            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }

            if (cookies == null)
            {
                httpWebResponse = request.GetResponse() as HttpWebResponse;
            }
            else
            {
                request.CookieContainer = new CookieContainer();

                request.CookieContainer.Add(cookies);

                httpWebResponse = request.GetResponse() as HttpWebResponse;
            }

            return httpWebResponse;
        }
    }
}