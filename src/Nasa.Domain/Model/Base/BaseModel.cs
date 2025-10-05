using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Nasa.Domain.Model;

[DataContract]
public class BaseModel
{
    public BaseModel()
    {
        Pagination = new Pagination();
    }

    public BaseModel(string defaultFilter, string newOrderBy)
    {
        Pagination = new Pagination();
        DefaultFilter = defaultFilter;
        NewOrderBy = newOrderBy;
    }

    [JsonIgnore]
    public Pagination Pagination { get; set; }

    [JsonIgnore]
    public string DefaultFilter { get; set; }

    [JsonIgnore]
    public string NewOrderBy { get; set; }

    [JsonIgnore]
    public string StartDate { get; set; }

    [JsonIgnore]
    public string EndDate { get; set; }

    public void SetPagination(long? pageNum, long? itemsPerPage)
    {
        this.Pagination = new Pagination(pageNum, itemsPerPage);
    }
}