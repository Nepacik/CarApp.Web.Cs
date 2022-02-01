using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CarAppWeb.Dtos;
using CarAppWeb.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CarAppWeb.Services
{
    public class BaseService
    {
        private readonly string _baseServiceUrl;
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;
        protected readonly IHttpContextAccessor HttpContextAccessor;

        public BaseService(IConfiguration configuration, HttpClient httpClient, string serviceUrl,
            IHttpContextAccessor httpContextAccessor)
        {
            _baseUrl = configuration["ApiConnection:baseUrl"];
            _baseServiceUrl = configuration["ApiConnection:baseUrl"] + serviceUrl;
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(15);
            HttpContextAccessor = httpContextAccessor;
        }

        protected async Task<ResponseState<TOut>> HandleRequest<TOut, TIn>(HttpServiceData httpServiceData, TIn body)
            where TIn : class
        {
            var uri = new Uri(_baseServiceUrl + httpServiceData.EndPointUrl);
            var requestMessage = new HttpRequestMessageBuilder(httpServiceData.HttpMethod, uri);
            requestMessage.SetContentBody(body);
            return await AttemptHttpRequest<TOut>(requestMessage);
        }

        protected async Task<ResponseState<TOut>> HandleQueryRequest<TOut>(HttpServiceData httpServiceData,
            Dictionary<string, string> query)
        {
            var requestUri = QueryHelpers.AddQueryString(_baseServiceUrl + httpServiceData.EndPointUrl, query);
            var uri = new Uri(requestUri);
            var requestMessage = new HttpRequestMessageBuilder(httpServiceData.HttpMethod, uri);
            return await AttemptHttpRequest<TOut>(requestMessage);
        }

        private async Task<ResponseState<TOut>> AttemptHttpRequest<TOut>(HttpRequestMessageBuilder httpRequestMessage)
        {
            var accessToken = await HttpContextAccessor.HttpContext.GetTokenAsync(nameof(TokenDto.AccessToken)) ?? "";

            if (!httpRequestMessage.Headers.Contains("Authorization"))
            {
                httpRequestMessage.Headers.Add("Authorization", "Bearer " + accessToken);
            }

            try
            {
                var response = await _httpClient.SendAsync(httpRequestMessage);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    if (httpRequestMessage.RequestUri != null &&
                        response.StatusCode == HttpStatusCode.Unauthorized &&
                        !httpRequestMessage.RequestUri.ToString().Contains("/Auth/RefreshToken"))
                    {
                        #region RequestTokenRegion

                        var refreshTokenAttpempt = await TryToRefreshToken();
                        if (refreshTokenAttpempt is ResponseState<TokenDto>.Error error)
                        {
                            if (error.HttpStatusCode == HttpStatusCode.Unauthorized)
                            {
                                await HttpContextAccessor.HttpContext.SignOutAsync();
                            }

                            return new ResponseState<TOut>.Error(error.Message, error.HttpStatusCode);
                        }

                        if (refreshTokenAttpempt is ResponseState<TokenDto>.Data data)
                        {
                            if (await TokenStorageProvider.SetTokenClaims(data.Content, HttpContextAccessor))
                            {
                                var newHttpRequest = new HttpRequestMessageBuilder(httpRequestMessage.Method,
                                    httpRequestMessage.RequestUri);
                                newHttpRequest.SetContentBody(httpRequestMessage.Content);
                                newHttpRequest.Headers.Add("Authorization", "Bearer " + data.Content.AccessToken);
                                return await AttemptHttpRequest<TOut>(newHttpRequest);
                            }

                            return new ResponseState<TOut>.UnknownError();
                        }

                        return new ResponseState<TOut>.UnknownError();

                        #endregion
                    }

                    return new ResponseState<TOut>.Error(result, response.StatusCode);
                }

                var jsonConverted = JsonConvert.DeserializeObject<TOut>(result);
                return new ResponseState<TOut>.Data(jsonConverted);
            }
            catch (Exception e)
            {
                return new ResponseState<TOut>.UnknownError();
            }
        }


        private async Task<ResponseState<TokenDto>> TryToRefreshToken()
        {
            var request = new HttpRequestMessageBuilder(HttpMethod.Post, new Uri(_baseUrl + "Auth/RefreshToken"));
            request.SetContentBody(new RefreshTokenDto
            {
                token = await HttpContextAccessor.HttpContext.GetTokenAsync(nameof(TokenDto.RefreshToken))
            });

            return await AttemptHttpRequest<TokenDto>(request);
        }
    }
}