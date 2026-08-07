using System.Text.Json.Serialization;

namespace ConfigSync.Adapters.In.Rest.Models.Responses;

[JsonConverter(typeof(JsonStringEnumConverter<CreateDeviceOutcome>))]
public enum CreateDeviceOutcome
{
    Created,
    AlreadyExists
}
