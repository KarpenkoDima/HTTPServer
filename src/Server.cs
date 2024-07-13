using codecrafters_http_server.src;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Data;

// You can use print statements as follows for debugging, they'll be visible when running tests.
Console.WriteLine("Logs from your program will appear here!");

// Uncomment this block to pass the first stage
TcpListener server = new TcpListener(IPAddress.Any, 4221);
server.Start();
/********************************************/
Router router = new Router();
router.AddRoute("GET", "/", (req, _) =>
{
    var response = new HttpResponse();
    response.StatusCode = HttpStatusCode.OK;
    return response;
});
router.AddRoute("GET", "/index.html", (req, _) =>
{
    var response = new HttpResponse();
    response.StatusCode = HttpStatusCode.OK;
    return response;
});
router.AddRoute("GET", "/echo/{src}", (req, _) =>
{
    var response = new HttpResponse();
    response.SetPlainTextContent(Path.GetFileName(req.Path));
    return response;
});
router.AddRoute("GET", "/user-agent", (req, _) =>
{
    var response = new HttpResponse();
    foreach (var header in req.Headers)
    {
        if (header.Key.ToUpper() == "USER-AGENT")
        {
            response.SetPlainTextContent(header.Value);
        }
    }
    //response.Headers["Content-Length"] = response.Body.Length.ToString();
    return response;
});

string data = string.Empty;
byte[] bytes = new byte[1024];
int bytesRec;
HttpRequest request = new HttpRequest();
HttpResponse response;

while (true)
{
    var socket = server.AcceptSocket(); // wait for client    

	while((bytesRec = socket.Receive(bytes)) != 0){
		data = Encoding.ASCII.GetString(bytes, 0, bytesRec);
		request.ParseRequest(data);
        response = router.Route(request);
        socket.Send(response.GetFullResponse());
    }
}