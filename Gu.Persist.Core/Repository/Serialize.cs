namespace Gu.Persist.Core
{
    using System.IO;

    /// <summary>
    /// Serialization logic.
    /// </summary>
    public abstract class Serialize<TSettings>
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
        public abstract void ToStream<T>(T item, Stream stream, TSettings settings);

        /// <summary>
        /// Deserialize <paramref name="stream"/> to an instance of <typeparamref name="T"/>
        /// </summary>
        public abstract T FromStream<T>(Stream stream);
    }
}