using System;
using System.Runtime.Serialization;

namespace Laborator5_PSSC.Domain.Models
{
    [Serializable]
    internal class InvalidProductQuantityException : Exception
    {
        public InvalidProductQuantityException()
        {
        }

        public InvalidProductQuantityException(string? message) : base(message)
        {
        }

        public InvalidProductQuantityException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidProductQuantityException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

}