using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Nasa.Domain.DTOs;

public class ProbabilidadesComportamento
{
    [JsonPropertyName("busca")]
    public double Busca { get; set; }

    [JsonPropertyName("forrageando")]
    public double Forrageando { get; set; }

    [JsonPropertyName("transitando")]
    public double Transitando { get; set; }
}