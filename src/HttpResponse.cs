using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafters_http_server.src
{
    public class HTTPResponse
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public string Headers { get; set; }
        public string Body { get; set; }

        public HTTPResponse(int statusCode, string statusMessage, string headers, string body)
        {
            StatusCode = statusCode;
            StatusMessage = statusMessage;
            Headers = headers;
            Body = body;
        }

        public string Serialize()
        {
            return $"HTTP/1.1 {StatusCode} {StatusMessage}\r\n{Headers}\r\n\r\n{Body}";
        }
    }
}
