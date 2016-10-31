namespace Gu.Persist.NewtonsoftJson
{
    using Newtonsoft.Json;

    interface IJsonRepositorySetting
    {
        JsonSerializerSettings JsonSerializerSettings { get; }
    }
}