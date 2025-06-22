using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

public abstract class Declaration
{
    public required string Id { get; set; }

    public static Declaration? ParseNullableFromJ(JObject j, Dictionary<string, object> astNodeDict) =>
        j.Value<string>("kind") is null ? null : ParseFromJ(j, astNodeDict);
    
    public static Declaration ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as Declaration)
                .Expect("Failed to cast existing AST node to Declaration.", j);
        }
        
        var kind = j.Value<string>("kind")
            .Expect("Cannot find the kind of the declaration.", j);
        if (DeclaredTypes.DeclarationTypes.TryGetValue(kind, out var type))
        {
            return (type.GetMethod(nameof(ParseFromJ))?.Invoke(null, [j, astNodeDict]) as Declaration)
                .Expect("Failed to parse Declaration from JObject.", j);
        }
        throw new NotSupportedException($"Unsupported Declaration kind: {kind}.\nJSON content: {j}");
    }
    
    public abstract GraphNode AddToGraph(AdjacencyGraph<GraphNode, GraphEdge> graph,
        Dictionary<string, GraphNode> astNodeDict);
}