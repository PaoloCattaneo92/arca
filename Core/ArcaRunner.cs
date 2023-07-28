using Serilog;

namespace Arca.Core;

public class ArcaRunner {

    private readonly ArcaDefinition definition;

    public ArcaRunner(ArcaDefinition definition)
    {
        this.definition = definition;
    }

    public void Run() {
        var client = new HttpClient();
        Log.Logger.Information("{name} start running", nameof(ArcaRunner));
        foreach(var call in definition.Calls) {
            Log.Logger.Debug("Calling onA: {method} {url}... ");
            try {
                var request = new HttpRequestMessage(call.OnA.Method, call.OnA.Url);
                if(!string.IsNullOrWhiteSpace(call.OnA.Body)) {
                    request.Content = new StringContent(call.OnA.Body);
                }

                var response = client.Send(request);
            }
            catch(Exception ex) {

            }
        }
    }
}