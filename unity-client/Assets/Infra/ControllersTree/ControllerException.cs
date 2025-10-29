using System;

namespace Infra.ControllersTree
{
    public class ControllerException : Exception
    {
        public ControllerException(string message) : base(message)
        {
        }

        public ControllerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}