using System;
using System.Net;

namespace Application.Errors
{
    public class RestExeption : Exception
    {
        public RestExeption(HttpStatusCode code,object errors=null)
        {
            Code = code;
            Errors = errors;
        }

        public HttpStatusCode Code { get; }
        public object Errors { get; }
    }
}