using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafters_http_server.src
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;



    public class Route
    {
        public string Template { get; private set; }
        public string HttpMethod { get; private set; }
        public Func<HttpRequest, Dictionary<string, string>, HttpResponse> Handler { get; private set; }
        private Regex RouteRegex { get; set; }
        private List<string> ParameterNames { get; set; }

        public Route(string httpMethod, string template, Func<HttpRequest, Dictionary<string, string>, HttpResponse> handler)
        {
            HttpMethod = httpMethod.ToUpper();
            Template = template;
            Handler = handler;
            ParameterNames = new List<string>();
            InitializeRouteRegex();
        }

        private void InitializeRouteRegex()
        {
            string pattern = Regex.Replace(Template, @"{(\w+)}", match =>
            {
                ParameterNames.Add(match.Groups[1].Value);
                return @"[(\w+)]*";
            });
            RouteRegex = new Regex($"^{pattern}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public bool Matches(string path, string method)
        {
            return RouteRegex.IsMatch(path) && HttpMethod == method.ToUpper();
        }

        public HttpResponse Execute(HttpRequest request)
        {
            var match = RouteRegex.Match(request.Path);
            var parameters = new Dictionary<string, string>();

            for (int i = 0; i < ParameterNames.Count; i++)
            {
                parameters[ParameterNames[i]] = match.Groups[i + 1].Value;
            }

            return Handler(request, parameters);
        }
    }

}
