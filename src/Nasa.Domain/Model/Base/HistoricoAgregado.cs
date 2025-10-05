using System.Runtime.Serialization;

namespace Nasa.Domain.Model;

[DataContract]
public class HistoricoAgregado : BaseModel
{
    public HistoricoAgregado()
    {
    }
    
    public HistoricoAgregado(string defaultFilter, string newOrderBy) : base(defaultFilter, newOrderBy)
    {
    }
    
    [DataMember(Name = "id")]
    public int Id { get; set; }
    
    [DataMember(Name = "data")]
    public DateTime? Data { get; set; }
    
    [DataMember(Name = "hora")]
    public int? Hora { get; set; }
    
    [DataMember(Name = "lat_media")]
    public double? LatMedia { get; set; }
    
    [DataMember(Name = "lon_media")]
    public double? LonMedia { get; set; }
    
    [DataMember(Name = "geom_media")]
    public string GeomMedia { get; set; }
    
    [DataMember(Name = "p_forrageio_media")]
    public float? PForrageioMedia { get; set; }
    
    [DataMember(Name = "comportamento_predominante")]
    public int? ComportamentoPredominante { get; set; }
    
    [DataMember(Name = "chlor_a_media")]
    public float? ChlorAMedia { get; set; }
    
    [DataMember(Name = "ssha_media")]
    public float? SshaMedia { get; set; }
    
    [DataMember(Name = "total_registros")]
    public int? TotalRegistros { get; set; }
    
    [DataMember(Name = "created_at")]
    public DateTime? CreatedAt { get; set; }
}

