namespace Gu.Persist.Core
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    /// <summary>
    /// This is thrown when restore fails.
    /// The exception that was thrown in restore is the inner exception.
    /// </summary>
    [Serializable]
#pragma warning disable CA1032 // Implement standard exception constructors
    public class RestoreException : IOException
#pragma warning restore CA1032 // Implement standard exception constructors
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
        /// Gets the exception that was thrown during the failed save the triggered the restore.
        /// </summary>
        public Exception SaveException { get; }

        /// <inheritdoc/>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(nameof(this.SaveException), this.SaveException, typeof(Exception));
            base.GetObjectData(info, context);
        }
    }
}