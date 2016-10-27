#pragma warning disable 1573
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
        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryRepository"/> class.
        /// Uses <see cref="Directories.Default"/>
        /// </summary>
        public BinaryRepository()
            : this(Directories.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryRepository"/> class.
        /// It will use BinaryRepositorySettings.DefaultFor(directory) as settings.
        /// </summary>
        public BinaryRepository(DirectoryInfo directory)
            : base(directory, () => BinaryRepositorySettings.DefaultFor(directory))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryRepository"/> class.
        /// If <paramref name="directory"/> contains a settings file it is read and used.
        /// If not a new default setting is created and saved.
        /// </summary>
        /// <param name="settingsCreator">Creates settings if file is missing</param>
        public BinaryRepository(DirectoryInfo directory, Func<BinaryRepositorySettings> settingsCreator)
            : base(directory, settingsCreator)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryRepository"/> class.
        /// </summary>
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