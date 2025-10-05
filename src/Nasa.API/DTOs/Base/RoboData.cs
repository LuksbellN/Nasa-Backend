using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Nasa.Domain.DTOs;

public class RoboData
{
    [JsonPropertyName("inputs")]
    public InputData Inputs { get; set; }

    [JsonPropertyName("outputs")]
    public OutputData Outputs { get; set; }
}