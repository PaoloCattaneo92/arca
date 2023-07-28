namespace Arca.Core;

public class CallDefinition {
    public CallDefinition()
    {
    }

    public CallDefinition(int line, HttpMethod method, string url, string? body)
    {
        Line = line;
        Method = method;
        Url = url;
        Body = body;
    }

    public int Line { get; set; }
    public HttpMethod Method { get; set; }
    public string Url { get; set; }
    public string? Body { get; set; }


    

}