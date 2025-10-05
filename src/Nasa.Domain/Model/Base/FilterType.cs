using System.Reflection;
using System.Runtime.Serialization;

namespace Nasa.Domain.Model;

[DataContract]
public class FilterType : BaseModel
{
    public FilterType()
    {

    }

    public FilterType(string defFilter, string newOrderBy) : base(defFilter, newOrderBy)
    {

    }

    [DataMember(Name = "code")]
    public string Code { get; set; }

    [DataMember(Name = "record_id")]
    public string RecordId { get; set; }

    [DataMember(Name = "description")]
    public string Description { get; set; }

    /// <summary>
    /// Used to filter results that require joins between tables.
    /// </summary>
    public string IdFilter { get; set; }

    /// <summary>
    /// Used to filter results by generic flags.
    /// </summary>
    public string FlgFilter { get; set; }
}