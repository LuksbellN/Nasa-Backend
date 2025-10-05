using System.Runtime.Serialization;

namespace Nasa.Domain.Model;

[DataContract]
public class RastreamentoTubaroes: BaseModel
{
    public RastreamentoTubaroes()
    {
    }
    
    public RastreamentoTubaroes(string defaultFilter, string newOrderBy) : base(defaultFilter, newOrderBy)
    {
    }
    
    [DataMember(Name = "id")]
    public int Id { get; set; }
    
    [DataMember(Name = "time")]
    public string Time { get; set; }
    
    [DataMember(Name = "lat")]
    public int Lat { get; set; }
    
    [DataMember(Name = "lon")]
    public int Lon { get; set; }
    
    [DataMember(Name = "temp_cc")]
    public int TempCc { get; set; }
    
    [DataMember(Name = "p_forrageio")]
    public decimal PForrageio { get; set; }
    
    [DataMember(Name = "comportamento")]
    public int Comportamento { get; set; }
    
    [DataMember(Name = "chlor_a_ambiente")]
    public decimal ChlorAAmbiente { get; set; }
    
    [DataMember(Name = "ssha_ambiente")]
    public decimal SshaAmbiente { get; set; }
    
    [DataMember(Name = "geometria")]
    public string Geometria { get; set; }
}