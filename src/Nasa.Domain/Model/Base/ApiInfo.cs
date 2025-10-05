using System.Reflection;
using System.Runtime.Serialization;

namespace Nasa.Domain.Model;

[DataContract]
public class ApiInfo
{
    [DataMember(Name = "version")]
    public string Version { get; set; }

    [DataMember(Name = "dateVersion")]
    public string DateVersion { get; set; }

    [DataMember(Name = "dateServer")]
    public string DateServer { get; set; }
    
    [DataMember(Name = "databaseName" )]
    public string DatabaseName { get; set; }

    [DataMember(Name = "userDatabaseName" )]
    public string UserDatabaseName { get; set; }

    [DataMember(Name = "dateDatabase" )]
    public string DateDatabase { get; set; }

    public ApiInfo(Assembly assembly)
    {
        string name = assembly.FullName;
        AssemblyName asmName = new AssemblyName(name);

        this.Version =
            string.Format("{0:00}.{1:00}.{2:00}.{3:0000}",
                asmName.Version.Major,
                asmName.Version.Minor,
                asmName.Version.Build,
                asmName.Version.MinorRevision
            );

        var descriptionAttribute = assembly
            .GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)
            .OfType<AssemblyDescriptionAttribute>()
            .FirstOrDefault();

        if (descriptionAttribute != null && descriptionAttribute.Description.Split('-').Count() > 1)
            this.DateVersion = descriptionAttribute.Description.Split('-')[1].Trim();

    }
}