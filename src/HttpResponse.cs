using System;
using System.Collections.Generic;
using System.IO.Compression;
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

        private Dictionary<HttpStatusCode, string> mapper = new Dictionary<HttpStatusCode, string>
    {
        { HttpStatusCode.OK, "OK" },
        { HttpStatusCode.Created, "Created" },
        { HttpStatusCode.NoContent, "No Content" },
        { HttpStatusCode.BadRequest, "Bad Request" },
        { HttpStatusCode.Unauthorized, "Unauthorized" },
        { HttpStatusCode.Forbidden, "Forbidden" },
        { HttpStatusCode.NotFound, "Not Found" },
        { HttpStatusCode.InternalServerError, "Internal Server Error" }
        };
        public HttpResponse()
        {
            Headers = new Dictionary<string, string>();
            StatusCode = HttpStatusCode.OK;
            //ContentType = string.Empty;
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
        public void SetOctetStreamContent(string octets)
        {
            Body = Encoding.UTF8.GetBytes(octets);
            ContentType = "application/octet-stream";
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
        public string /*byte[]*/ GetFullResponse()
        {
            var responseBuilder = new StringBuilder();
            responseBuilder.Append($"HTTP/1.1 {(int)StatusCode} {mapper[StatusCode]}");
            responseBuilder.Append("\r\n");
            responseBuilder.Append($"Content-Type: {ContentType}");
            responseBuilder.Append("\r\n");
            responseBuilder.Append($"Content-Length: {Body.Length}");
            responseBuilder.Append("\r\n");

            foreach (var header in Headers)
            {
                responseBuilder.Append($"{header.Key}: {header.Value}");
                responseBuilder.Append("\r\n");
            }

            responseBuilder.Append("\r\n");

            var headerBytes = Encoding.ASCII.GetString(Body);
            /* var fullResponse = new byte[headerBytes.Length + Body.Length];
             Buffer.BlockCopy(headerBytes, 0, fullResponse, 0, headerBytes.Length);
             Buffer.BlockCopy(Body, 0, fullResponse, headerBytes.Length, Body.Length);
            */
            responseBuilder.Append(headerBytes);
            return responseBuilder.ToString(); // fullResponse;
        }
        public void CreateCompressedResponse()
        {
            byte[] buffer = Body;
            MemoryStream ms = new MemoryStream();

            using (GZipStream gzip = new GZipStream(ms, CompressionMode.Compress, true))
            {
                gzip.Write(buffer, 0, buffer.Length);
            }
            ms.Position = 0;
            Body = new byte[ms.Length];
            ms.Read(Body, 0, Body.Length);
            AddHeader("Content-Encoding", Encoding.UTF8.ToString());
            AddHeader("Content-Length64", Body.Length.ToString());
            AddHeader("Content-Encoding", "gzip");
        }
    }
}
