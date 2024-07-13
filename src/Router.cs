using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafters_http_server.src
{
    public class Router
    {
        private List<Route> routes;

        public Router()
        {
            routes = new List<Route>();
        }

        public void AddRoute(string method, string template, Func<HttpRequest, Dictionary<string, string>, HttpResponse> handler)
        {
            routes.Add(new Route(method, template, handler));
        }

        public HttpResponse Route(HttpRequest request)
        {

            foreach (var route in routes)
            {
                if (route.Matches(request.Path, request.Method))
                {
                    return route.Execute(request);
                }
            }

            return NotFound();
        }

        private HttpResponse NotFound()
        {
            var response = new HttpResponse();
            response.StatusCode = System.Net.HttpStatusCode.NotFound;
            response.SetPlainTextContent("404 Not Found");
            return response;
        }
    }
}
