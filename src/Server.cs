using codecrafters_http_server.src;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;

// You can use print statements as follows for debugging, they'll be visible when running tests.
Console.WriteLine("Logs from your program will appear here!");

// Uncomment this block to pass the first stage
TcpListener server = new TcpListener(IPAddress.Any, 4221);
server.Start();
while (true)
{
    var socket = server.AcceptSocket(); // wait for client    
    string data = string.Empty;
    byte[] bytes = new byte[1024];
    int bytesRec;
	HttpRequest request;
	HTTPResponse response;
	while((bytesRec = socket.Receive(bytes)) != 0){
		data = Encoding.ASCII.GetString(bytes, 0, bytesRec);
		request = HttpRequest.ParseHttpRequest(data);
		if (request != null)
		{
			
			if(Path.GetDirectoryName(request.Path) == "\\echo" || Path.GetDirectoryName(request.Path) == "/echo")
			{ 
				string headers = $"Content-Type: text/plain\r\nContent-Length: {Path.GetFileName(request.Path).Length}";
				response = new HTTPResponse(200, "OK", headers, Path.GetFileName(request.Path));
				socket.Send(Encoding.ASCII.GetBytes(response.Serialize()));
			}
			else 
			{
				response = new HTTPResponse(404, "Not Found", string.Empty, string.Empty);
				socket.Send(Encoding.ASCII.GetBytes(response.Serialize()));
			}
		}
	}
}