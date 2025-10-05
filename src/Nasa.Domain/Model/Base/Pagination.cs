using System.Runtime.Serialization;

namespace Nasa.Domain.Model;

[DataContract]
public class Pagination
{
    public Pagination()
    {

    }

    public Pagination(long? pageNum, long? itemsPerPage)
    {
        if (pageNum == null || itemsPerPage == null)
            return;

        this.PageNum = pageNum;
        this.ItemsPerPage = itemsPerPage;

        this.StartRecordNumber = ((this.PageNum - 1) * this.ItemsPerPage) + 1;
        this.EndRecordNumber = ((this.PageNum - 1) + this.ItemsPerPage);
    }


    [DataMember(Name = "startRecordNumber")]
    public long? StartRecordNumber { get; set; }

    [DataMember(Name = "endRecordNumber")]
    public long? EndRecordNumber { get; set; }

    [DataMember(Name = "itemsPerPage")]
    public long? ItemsPerPage { get; set; }

    [DataMember(Name = "pageNum")]
    public long? PageNum { get; set; }

    [DataMember(Name = "totalRecords")]
    public long? TotalRecords { get; set; }

    public long? TotalPages
    {
        get
        {
            if (TotalRecords == null || ItemsPerPage == null)
                return null;

            decimal division = ((decimal)TotalRecords.Value / ItemsPerPage.Value);
            var ceiling = Math.Ceiling(division);
            return Convert.ToInt64(ceiling);
        }
    }


    public bool UsesPagination()
    {
        return StartRecordNumber != null && EndRecordNumber != null;
    }

}