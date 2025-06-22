using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

public abstract class Statement : IGraphNode
{
    public required string Id { get; set; }

    public static Statement? ParseNullableFromJ(JObject j, Dictionary<string, object> astNodeDict) =>
        j.Value<string>("kind") is null ? null : ParseFromJ(j, astNodeDict);

    public static Statement ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as Statement)
                .Expect("Failed to cast existing AST node to Statement.", j);
        }
        
        var kind = j.Value<string>("kind")
            .Expect("Cannot find the kind of the statement", j);
        if (DeclaredTypes.StatementTypes.TryGetValue(kind, out var type))
        {
            return (type.GetMethod(nameof(ParseFromJ))?.Invoke(null, [j, astNodeDict]) as Statement)
                .Expect("Internal error: Cannot parse statement from JSON.", j);
        }
        throw new NotSupportedException($"Unsupported statement type: {kind}.\nJSON content: {j}");
    }

    public abstract GraphNode AddToGraph(AdjacencyGraph<GraphNode, GraphEdge> graph,
        Dictionary<string, GraphNode> astNodeDict);
}