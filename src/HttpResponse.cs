using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace codecrafters_http_server.src
{
    public class HttpResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string ContentType { get; set; }
        public Dictionary<string, string> Headers { get; private set; }
        public byte[] Body { get; set; }

        public HttpResponse()
        {
            Headers = new Dictionary<string, string>();
            StatusCode = HttpStatusCode.OK;
            ContentType = string.Empty;
            Body = new byte[0];
        }

        public void AddHeader(string key, string value)
        {
            Headers[key] = value;
        }

        public void SetJsonContent(string json)
        {
            Body = Encoding.UTF8.GetBytes(json);
            ContentType = "application/json";
        }

        public void SetHtmlContent(string html)
        {
            Body = Encoding.UTF8.GetBytes(html);
            ContentType = "text/html";
        }

        public void SetPlainTextContent(string text)
        {
            Body = Encoding.UTF8.GetBytes(text);
            ContentType = "text/plain";
        }
        [Obsolete]
        public string GetOK()
        {
            return $"HTTP/1.1 {StatusCode} OK\r\n{Headers}\r\n\r\n";
        }
        public byte[] GetFullResponse()
        {
            var responseBuilder = new StringBuilder();
            responseBuilder.AppendLine($"HTTP/1.1 {(int)StatusCode} {StatusCode}");
            responseBuilder.AppendLine($"Content-Type: {ContentType}");
            responseBuilder.AppendLine($"Content-Length: {Body.Length}");

            foreach (var header in Headers)
            {
                responseBuilder.AppendLine($"{header.Key}: {header.Value}");
            }

            responseBuilder.AppendLine();
            string crlf = "\r\n";
            byte[] crlfBytes = Encoding.ASCII.GetBytes(crlf);
            Body = Body.Concat( crlfBytes ).ToArray();
            var headerBytes = Encoding.ASCII.GetBytes(responseBuilder.ToString());
            var fullResponse = new byte[headerBytes.Length + Body.Length];
            Buffer.BlockCopy(headerBytes, 0, fullResponse, 0, headerBytes.Length);
            Buffer.BlockCopy(Body, 0, fullResponse, headerBytes.Length, Body.Length);

            return fullResponse;
        }
    }
}
