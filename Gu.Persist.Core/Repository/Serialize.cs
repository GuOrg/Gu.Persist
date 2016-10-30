namespace Gu.Persist.Core
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Serialization logic.
    /// </summary>
    public abstract class Serialize<TSettings> : ICloner
    {
        /// <summary>
        /// Serialize <paramref name="item"/> to a Stream
        /// </summary>
        /// <param name="item">
        /// This can be either an instance of <typeparamref name="T"/> or a <see cref="Stream"/>
        /// </param>
        public abstract Stream ToStream<T>(T item);

        /// <summary>
        /// Serialize <paramref name="item"/> to a Stream
        /// </summary>
        /// <param name="item">
        /// This can be either an instance of <typeparamref name="T"/> or a <see cref="Stream"/>
        /// </param>
        /// <param name="stream">
        /// The stream to write serialized data to.
        /// </param>
        /// <param name="settings">
        /// The settings to use when serializing.
        /// </param>
        public abstract void ToStream<T>(T item, Stream stream, TSettings settings);

        /// <summary>
        /// Deserialize <paramref name="stream"/> to an instance of <typeparamref name="T"/>
        /// </summary>
        public abstract T FromStream<T>(Stream stream);

        /// <inheritdoc />
        public abstract T Clone<T>(T item);

        /// <summary>
        /// Gets the comparer to use when checking <see cref="IDirty.IsDirty{T}(T)"/>
        /// </summary>
        public abstract IEqualityComparer<T> DefaultStructuralEqualityComparer<T>();
    }
}