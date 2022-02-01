using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace CarAppWeb.Http
{
    public class HttpRequestMessageBuilder: HttpRequestMessage
    {
        public HttpRequestMessageBuilder(HttpMethod httpMethod, Uri uri) : base(httpMethod, uri)
        {
            
        }

        public void SetContentBody(object body)
        {
            var json = JsonConvert.SerializeObject(body);
            StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            Content = httpContent;
        }
    }
}