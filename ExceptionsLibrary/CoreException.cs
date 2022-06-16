using Newtonsoft.Json;

namespace ExceptionsLibrary;

[Serializable]
public class CoreException : Exception
{
    public CoreException()
    {
        UserMessage = string.Empty;
        TechnicalMessage = string.Empty;
    }

    public CoreException(string message) : base(message)
    {
        UserMessage = string.Empty;
        TechnicalMessage = string.Empty;
    }

    public CoreException(string message, Exception inner) : base(message, inner)
    {
        UserMessage = string.Empty;
        TechnicalMessage = string.Empty;
    }

    public CoreException(string message, int statusCode)
    {
        UserMessage = message;
        StatusCode = statusCode;
        TechnicalMessage = string.Empty;
    }

    public int StatusCode { get; set; }
    public string UserMessage { get; set; }
    public string TechnicalMessage { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}