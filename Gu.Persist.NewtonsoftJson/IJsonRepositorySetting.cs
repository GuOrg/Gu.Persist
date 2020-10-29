namespace Gu.Persist.NewtonsoftJson
{
    using Newtonsoft.Json;

    /// <summary>
    /// Used for constraining <see cref="Serialize{TSetting}"/>.
    /// </summary>
    public interface IJsonRepositorySetting
    {
        /// <summary>
        /// Gets get the settings controlling serialization of JSON.
        /// </summary>
        JsonSerializerSettings JsonSerializerSettings { get; }
    }
}