using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Statement("ContinueStmt")]
public class ContinueStatement : Statement
{
    public new static ContinueStatement ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as ContinueStatement)
                .Expect("Failed to cast existing AST node to ContinueStatement.", j);
        }
        
        ContinueStatement node = new()
        {
            Id = id
        };
        
        astNodeDict[id] = node;
        return node;
    }
    
    public override GraphNode AddToGraph(AdjacencyGraph<GraphNode, GraphEdge> graph, Dictionary<string, GraphNode> astNodeDict)
    {
        if (astNodeDict.TryGetValue(Id, out var existingNode))
        {
            return existingNode;
        }
        
        var node = new GraphNode
        {
            Id = Id,
            Label = "[Continue]"
        };
        graph.AddVertex(node);
        
        astNodeDict[Id] = node;
        return node;
    }
}