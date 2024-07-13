using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafters_http_server.src
{

    public class HttpRequest
    {
        public const int MaxBuffer = 4096;

        public string Method { get; set; }
        public string Path { get; set; }
        public string Version { get; set; }
        public string Headers { get; set; }
        public string Body { get; set; }

        public HttpRequest()
        {
            Method = string.Empty;
            Path = string.Empty;
            Version = string.Empty;
            Headers = string.Empty;
            Body = string.Empty;
        }


        public static HttpRequest ParseHttpRequest(string rawRequest)
        {
            var request = new HttpRequest();

            string[] lines = rawRequest.Split(new[] { "\r\n" }, StringSplitOptions.None);

            if (lines.Length > 0)
            {
                string[] firstLineParts = lines[0].Split(' ');
                if (firstLineParts.Length >= 3)
                {
                    request.Method = firstLineParts[0];
                    request.Path = firstLineParts[1];
                    request.Version = firstLineParts[2];
                }
            }

            int headerEndIndex = Array.IndexOf(lines, string.Empty);
            if (headerEndIndex > 0)
            {
                request.Headers = string.Join("\r\n", lines, 1, headerEndIndex - 1);
                if (headerEndIndex < lines.Length - 1)
                {
                    request.Body = string.Join("\r\n", lines, headerEndIndex + 1, lines.Length - headerEndIndex - 1);
                }
            }
            else
            {
                request.Headers = string.Join("\r\n", lines, 1, lines.Length - 1);
            }

            return request;
        }
    }
}
