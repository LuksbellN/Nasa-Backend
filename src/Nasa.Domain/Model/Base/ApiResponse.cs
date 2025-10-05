using System.Runtime.Serialization;

namespace Nasa.Domain.Model;

[DataContract]
public class ApiResponse
{
    [DataMember(Name = "success")]
    public bool Success { get; set; }
    
    [DataMember(Name = "message")]
    public string Message { get; set; }
    
    [DataMember(Name = "result")]
    public object Result { get; set; }
    
} 