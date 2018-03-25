using System;

namespace Toggle.Client
{
    public class UnsuccessfulResponseFromApiException : Exception
    {
        public int StatusCode { get; internal set; }
        public UnsuccessfulResponseFromApiException(int statusCode) : base(string.Format("Http status {0}", statusCode))
        {
            
        }
    }
}