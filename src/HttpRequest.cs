using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafters_http_server.src
{

    public class HttpRequest
    {
        public string Method { get; set; }
        public string Path { get; set; }
        public string HttpVersion { get; set; }
        public Dictionary<string, string> Headers { get; private set; }
        public Dictionary<string, string> QueryParameters { get; private set; }
        public byte[] Body { get; set; }

        public HttpRequest()
        {
            Headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            QueryParameters = new Dictionary<string, string>();
        }

        public void ParseRequest(string requestString)
        {
            var lines = requestString.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            var firstLine = lines[0].Split(' ');

            Method = firstLine[0];
            var fullPath = firstLine[1];
            HttpVersion = firstLine[2];

            var pathParts = fullPath.Split('?');
            Path = pathParts[0];

            if (pathParts.Length > 1)
            {
                ParseQueryParameters(pathParts[1]);
            }

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    break;

                var headerParts = lines[i].Split(new[] { ':' }, 2);
                if (headerParts.Length == 2)
                {
                    Headers[headerParts[0].Trim()] = headerParts[1].Trim();
                }
            }

            // Парсинг тела запроса, если оно есть
            if (Headers.ContainsKey("Content-Length"))
            {
                int contentLength = int.Parse(Headers["Content-Length"]);
                Body = new byte[contentLength];               
                // Здесь нужно реализовать чтение тела запроса
                // нужно сверить размер тела запроса с Content-Length
                Buffer.BlockCopy(Encoding.ASCII.GetBytes(lines.Last()), 0, Body, 0, contentLength);
              
            }
        }

        private void ParseQueryParameters(string queryString)
        {
            var pairs = queryString.Split('&');
            foreach (var pair in pairs)
            {
                var keyValue = pair.Split('=');
                if (keyValue.Length == 2)
                {
                    QueryParameters[Uri.UnescapeDataString(keyValue[0])] = Uri.UnescapeDataString(keyValue[1]);
                }
            }
        }

        public string GetBodyAsString()
        {
            return Body != null ? Encoding.UTF8.GetString(Body) : string.Empty;
        }

        public override string ToString()
        {
            return $"{Method} {Path} {HttpVersion}";
        }
    }
}
