namespace Gu.Settings.RuntimeBinary
{
    using System.Collections.Generic;
    using System.IO;

    using Gu.Settings.Core;

    /// <summary>
    /// A repository reading and saving files using <see cref="System.Runtime.Serialization.Formatters.Binary.BinaryFormatter"/>
    /// </summary>
    public class BinaryRepository : Repository
    {
        /// <inheritdoc/>
        public BinaryRepository()
            : base(Directories.Default)
        {
        }

        /// <inheritdoc/>
        public BinaryRepository(DirectoryInfo directory)
            : base(directory)
        {
        }

        /// <inheritdoc/>
        public BinaryRepository(RepositorySettings settings)
            : base(settings)
        {
        }

        /// <inheritdoc/>
        protected override T FromStream<T>(Stream stream)
        {
            return BinaryHelper.FromStream<T>(stream);
        }

        /// <inheritdoc/>
        protected override Stream ToStream<T>(T item)
        {
            return BinaryHelper.ToStream(item);
        }

        /// <inheritdoc/>
        protected override IEqualityComparer<T> DefaultStructuralEqualityComparer<T>()
        {
            return BinaryEqualsComparer<T>.Default;
        }
    }
}