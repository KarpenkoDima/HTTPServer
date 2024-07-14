using codecrafters_http_server.src;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Data;
using System.Threading.Tasks;
using System.Reflection.PortableExecutable;

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

router.AddRoute("GET", "/files/{filename}", (req, _) =>
{
	var response = new HttpResponse();
	var file = Path.GetFileName(req.Path);
	var pathToFile = Path.Combine("/tmp/data/codecrafters.io/http-server-tester", file);
	if (Path.Exists(pathToFile))
	{
		string text = File.ReadAllText(pathToFile);
		response.SetOctetStreamContent(text);
	}
	else
	{
		response.StatusCode = System.Net.HttpStatusCode.NotFound;
		response.SetPlainTextContent("404 Not Found");
		
	}
	return response;
});

string data = string.Empty;
byte[] bytes = new byte[1024];
int bytesRec;
HttpRequest request = new HttpRequest();
HttpResponse response;
/*
TcpClient
while (true)
{
	TcpClient client = await server.AcceptTcpClientAsync();
	_ = HandleClientAsync(client);
}


async Task HandleClientAsync(TcpClient client)
{
	try
	{
		using (NetworkStream stream = client.GetStream())
		{
			byte[] buffer = new byte[1024];
			int bytesRead;

			while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
			{
				string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);					
				request.ParseRequest(message);
				response = router.Route(request);
				var responseBytes = Encoding.ASCII.GetBytes(response.GetFullResponse());				
				await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
			}
		}
	}
	catch (Exception ex)
	{
		Console.WriteLine($"Error: {ex.Message}");
	}
	finally
	{
		client.Close();
	}
}
*/
/* Socket client */
while (true)
{
		var socket = await server.AcceptSocketAsync(); // wait for client    
		Console.WriteLine("Accept connect");
		_= ProcessClientAsync(socket);
}

async Task ProcessClientAsync(Socket socket)
{
	string data = string.Empty;
	byte[] bytes = new byte[1024];
	int bytesRec;
	try
	{
		while ((bytesRec = await socket.ReceiveAsync(new ArraySegment<byte>(bytes), SocketFlags.None)) != 0)
		{
			data = Encoding.ASCII.GetString(bytes, 0, bytesRec);
			request.ParseRequest(data);
			response = router.Route(request);
			var msg = Encoding.ASCII.GetBytes(response.GetFullResponse());
			//string message = "HTTP/1.1 200 OK\r\n\r\n";
			//byte[] messageBytes = Encoding.ASCII.GetBytes(message);

			await socket.SendAsync(new ArraySegment<byte>(msg), SocketFlags.None);
		}
	}
	finally
	{
		socket.Close();
		socket.Dispose();
	}
}
