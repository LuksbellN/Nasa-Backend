using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Nasa.Domain.DTOs;

public class OutputData
{
    [JsonPropertyName("comportamento")]
    public string Comportamento { get; set; }

    [JsonPropertyName("probabilidades_comportamento")]
    public ProbabilidadesComportamento ProbabilidadesComportamento { get; set; }

    [JsonPropertyName("p_forrageio")]
    public double PForrageio { get; set; }
}