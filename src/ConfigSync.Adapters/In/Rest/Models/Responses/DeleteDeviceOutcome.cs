using System.Text.Json.Serialization;

namespace ConfigSync.Adapters.In.Rest.Models.Responses;

[JsonConverter(typeof(JsonStringEnumConverter<DeleteDeviceOutcome>))]
public enum DeleteDeviceOutcome
{
    Deleted,
    NotFound
}
