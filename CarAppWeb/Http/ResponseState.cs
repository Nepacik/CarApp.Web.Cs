using System.Net;

namespace CarAppWeb.Http
{
    public abstract class ResponseState<T>
    {
        public class Data : ResponseState<T>
        {
            public T Content { get; }

            public Data(T data)
            {
                Content = data;
            }
        }

        public class Error : ResponseState<T>
        {
            public string Message { get; }
            public HttpStatusCode HttpStatusCode { get; }

            public Error(string message, HttpStatusCode httpStatusCode)
            {
                Message = message;
                HttpStatusCode = httpStatusCode;
            }
        }

        public class UnknownError : ResponseState<T>
        {
            public UnknownError()
            {
            }
        }
    }
}