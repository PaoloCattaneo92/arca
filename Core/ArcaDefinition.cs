namespace Arca.Core;

public class ArcaDefinition {
    public Dictionary<string, Constant> Constants { get; }
    public List<CallPair> Calls { get; }

    public ArcaDefinition() {
        Calls = new List<CallPair>();
        Constants = new Dictionary<string, Constant>();
    }
}