using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;

namespace CarAppWeb.Http
{
    public class HttpServiceData
    {
        public string EndPointUrl { get;}
        public HttpMethod HttpMethod { get;}

        public object Body { get; set; }
        public string Param { get; set; }

        public HttpServiceData(string endPointUrl, HttpMethod httpMethod)
        {
            HttpMethod = httpMethod;
            EndPointUrl = endPointUrl;
        }
    }
}