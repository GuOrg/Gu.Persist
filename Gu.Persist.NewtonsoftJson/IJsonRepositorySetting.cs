namespace Gu.Persist.NewtonsoftJson
{
    using Newtonsoft.Json;

    /// <summary>
    /// Used for constraining <see cref="Serialize{TSetting}"/>.
    /// </summary>
    public interface IJsonRepositorySetting
    {
        /// <summary>
        /// Get the settings controlling serialization of JSON.
        /// </summary>
        JsonSerializerSettings JsonSerializerSettings { get; }
    }
}