namespace Gu.Persist.Core
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Serialization logic.
    /// </summary>
    /// <typeparam name="TSettings">The setting type.</typeparam>
    public abstract class Serialize<TSettings>
    {
        /// <summary>
        /// Serialize <paramref name="item"/> to a Stream.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="item">
        /// This can be either an instance of <typeparamref name="T"/> or a <see cref="Stream"/>.
        /// </param>
        /// <param name="settings">The settings to use when serializing.</param>
        /// <returns><paramref name="item"/> serialized.</returns>
        public abstract Stream ToStream<T>(T item, TSettings settings);

        /// <summary>
        /// Serialize <paramref name="item"/> to a Stream.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="item">
        /// This can be either an instance of <typeparamref name="T"/> or a <see cref="Stream"/>.
        /// </param>
        /// <param name="stream">
        /// The stream to write serialized data to.
        /// </param>
        /// <param name="settings">The settings to use when serializing.</param>
        public abstract void ToStream<T>(T item, Stream stream, TSettings settings);

        /// <summary>
        /// Deserialize <paramref name="stream"/> to an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to read from the file.</typeparam>
        /// <param name="stream">
        /// The stream to read from.
        /// </param>
        /// <param name="settings">The settings to use when serializing.</param>
        /// <returns>The deserialized instance.</returns>
        public abstract T FromStream<T>(Stream stream, TSettings settings);

        /// <summary>
        /// Serialize then deserialize <paramref name="item"/> to create a deep copy.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="item">The <see cref="T"/>.</param>
        /// <param name="settings">The <typeparamref name=""/> cref="TSettings"/>.</param>
        /// <returns>A deep copy.</returns>
        public abstract T Clone<T>(T item, TSettings settings);

        /// <summary>
        /// Gets the comparer to use when checking <see cref="IDirty.IsDirty{T}(T)"/>.
        /// </summary>
        /// <typeparam name="T">The type to read from the file.</typeparam>
        /// <param name="settings">The <typeparamref name="TSettings"/>.</param>
        /// <returns>A <see cref="IEqualityComparer{T}"/>.</returns>
        public abstract IEqualityComparer<T> DefaultStructuralEqualityComparer<T>(TSettings settings);
    }
}