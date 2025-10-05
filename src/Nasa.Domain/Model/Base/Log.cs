using System.Runtime.CompilerServices;

namespace Nasa.Domain.Model;

public class Log
{
    public long? IdLog { get; set; }

    public string DesObjeto { get; set; }

    public string DesOperacao { get; set; }

    public string ObsOperacao { get; set; }

    public string Username { get; set; }

    public DateTime DtaOperacao { get; set; }

    public string DesErro { get; set; }

    public void LogProccessBeingProduced(string obsOperacao, string username, [CallerMemberName] string desOperacao = "")
    {
        DesOperacao = desOperacao;
        ObsOperacao = obsOperacao;
        Username = username;
    }
}