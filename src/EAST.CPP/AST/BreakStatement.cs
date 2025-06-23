using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Statement("BreakStmt")]
public class BreakStatement : Statement
{
    public new static BreakStatement ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as BreakStatement)
                .Expect("Failed to cast existing AST node to BreakStatement.", j);
        }

        BreakStatement node = new()
        {
            Id = id
        };

        astNodeDict[id] = node;
        return node;
    }
    
    public override GraphNode AddToGraph(AdjacencyGraph<GraphNode, GraphEdge> graph, Dictionary<string, GraphNode> astNodeDict)
    {
        if (astNodeDict.TryGetValue(Id, out var existing))
        {
            return existing;
        }

        GraphNode node = new()
        {
            Id = Id,
            Label = "[Break]"
        };
        graph.AddVertex(node);
        
        astNodeDict[Id] = node;
        return node;
    }
}