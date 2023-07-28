namespace Arca.Core;

public record CallPair(
    string Name,
    CallDefinition OnA,
    CallDefinition OnB);