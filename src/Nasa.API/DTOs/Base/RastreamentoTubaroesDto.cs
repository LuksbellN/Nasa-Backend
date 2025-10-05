using System.Runtime.Serialization;

namespace Nasa.Domain.DTOs;

[DataContract]
public class RastreamentoTubaroesDto
{
    [DataMember(Name = "id")] public int Id { get; set; }

    [DataMember(Name = "tempo")] public DateTime? Tempo { get; set; }

    [DataMember(Name = "lat")] public double Lat { get; set; }

    [DataMember(Name = "lon")] public double Lon { get; set; }
    [DataMember(Name = "temp_cc")] public double TempCc { get; set; }
    [DataMember(Name = "p_forrageio")] public double PForrageio { get; set; }
    [DataMember(Name = "comportamento")] public string Comportamento { get; set; }

    [DataMember(Name = "chlor_a_ambiente")]
    public double ChlorAAmbiente { get; set; }

    [DataMember(Name = "ssha_ambiente")] public double SshaAmbiente { get; set; }
}