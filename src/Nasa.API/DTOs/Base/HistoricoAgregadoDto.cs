using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Nasa.Domain.DTOs;

[DataContract]
public class HistoricoAgregadoDto
{
    [DataMember(Name = "id")]
    public int Id { get; set; }
    
    [DataMember(Name = "data")]
    public DateTime? Data { get; set; }
    
    [DataMember(Name = "hora")]
    public int? Hora { get; set; }

    [JsonIgnore]
    public double? LatMedia { get; set; }
    
    [JsonIgnore]
    public double? LonMedia { get; set; }
    
    [JsonIgnore]
    public string GeomMedia { get; set; }
    
    [JsonIgnore]
    public float? PForrageioMedia { get; set; }
    
    [JsonIgnore]
    public int? ComportamentoPredominante { get; set; }
    
    [JsonIgnore]
    public float? ChlorAMedia { get; set; }
    
    [JsonIgnore]
    public float? SshaMedia { get; set; }
    
}

