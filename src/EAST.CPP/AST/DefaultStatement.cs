using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Expression("DefaultStmt")]
public class DefaultStatement : Statement
{
    public required Expression Inner { get; set; }
    
    public new static DefaultStatement ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as DefaultStatement)
                .Expect("Failed to cast existing AST node to DefaultStatement.", j);
        }
        
        var children = j.GetNullableChildren();
        DefaultStatement node = new()
        {
            Id = id,
            Inner = Expression.ParseFromJ(
                children.FirstOrDefault()
                    .Expect("Cannot find inner expression in DefaultStatement.", j),
                astNodeDict
            )
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
        
        GraphNode node = new()
        {
            Id = Id,
            Label = "[Default]"
        };
        graph.AddVertex(node);
        
        var innerNode = Inner.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new GraphEdge(node, innerNode));
        
        astNodeDict[Id] = node;
        return node;
    }
}