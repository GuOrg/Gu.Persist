namespace Gu.Settings.RuntimeBinary
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Gu.Settings.Core;

    /// <summary>
    /// A repository reading and saving files using <see cref="System.Runtime.Serialization.Formatters.Binary.BinaryFormatter"/>
    /// </summary>
    public class BinaryRepository : Repository<BinaryRepositorySettings>
    {
        /// <inheritdoc/>
        public BinaryRepository()
            : this(Directories.Default)
        {
        }

        /// <inheritdoc/>
        public BinaryRepository(DirectoryInfo directory)
            : base(directory, () => BinaryRepositorySettings.DefaultFor(directory))
        {
        }

        /// <inheritdoc/>
        public BinaryRepository(DirectoryInfo directory, Func<BinaryRepositorySettings> settingsCreator)
            : base(directory, settingsCreator)
        {
        }

        /// <inheritdoc/>
        public BinaryRepository(BinaryRepositorySettings settings)
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