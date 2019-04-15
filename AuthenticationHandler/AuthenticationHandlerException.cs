using System;

namespace AuthenticationHandler
{
    public class AuthenticationHandlerException : Exception
    {
        public AuthenticationHandlerException(string message) : base(message)
        {
        }
    }
}