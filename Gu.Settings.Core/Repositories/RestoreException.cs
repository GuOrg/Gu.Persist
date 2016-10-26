// ReSharper disable All
namespace Gu.Settings.Core
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;

    public class RestoreException : IOException
    {
        public RestoreException(Exception exception, Exception restoreException)
        {
            SaveException = exception;
            Exception = restoreException;
        }

        public Exception Exception { get; }

        public Exception SaveException { get; }

        protected RestoreException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}