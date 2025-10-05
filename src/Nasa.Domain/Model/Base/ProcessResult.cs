using System.Runtime.Serialization;
using Nasa.Resources;

namespace Nasa.Domain.Model;

[DataContract]
public class ProcessResult
{
        [DataMember(Name = "data")]
        public object Data { get; set; }

        [DataMember(Name = "pagination")]
        public Pagination Pagination { get; set; }
        
        public string ProcessMessage { get; set; }

        public Exception Error { get; set; }

        public bool Success { get; set; }
        
        public bool HaveWarnings { get; set; }

        public int HttpStatusCode { get; set; }

        public ProcessResult()
        {
            Success = true;
            ProcessMessage = AppStrings.INF_OperacaoEncerradaComSucesso;
        }

        public ProcessResult(Exception exception)
        {
            RecordException(exception);
        }

        public ProcessResult(string processMessage)
        {
            RecordException(processMessage);
        }

        public void RecordException(Exception exception)
        {
            RecordException(exception, null);
        }

        public void RecordException(string processMessage)
        {
            RecordException(null, processMessage);
        }

        public void RecordException(string processMessage, int httpStatusCode)
        {
            HttpStatusCode = httpStatusCode;
            RecordException(null, processMessage);
        }

        public void RecordException(Exception exception, string processMessage)
        {
            this.Success = false;
            this.Error = exception;
            this.ProcessMessage =
                (string.IsNullOrEmpty(processMessage)
                    ? exception.Message
                    : processMessage
                    );
        }
}