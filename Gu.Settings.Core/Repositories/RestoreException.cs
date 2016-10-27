namespace Gu.Settings.Core
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    /// <summary>
    /// This is thrown when restor fails.
    /// The exception that was thrown in restore is the inner exception.
    /// </summary>
    [Serializable]
    public class RestoreException : IOException
    {
        public RestoreException(Exception saveException, Exception innerException)
            : base("Restore failed", innerException)
        {
            this.SaveException = saveException;
        }

        protected RestoreException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.SaveException = (Exception)info.GetValue(nameof(this.SaveException), typeof(Exception));
        }

        /// <summary>
        /// The exception that was thrown during the failed save the triggered the restore.
        /// </summary>
        public Exception SaveException { get; }

        /// <inheritdoc/>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(this.SaveException), this.SaveException, typeof(Exception));
            base.GetObjectData(info, context);
        }
    }
}