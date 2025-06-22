using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Statement("ReturnStmt")]
public class ReturnStatement : Statement
{
    public required Expression Expression { get; set; }

    public new static ReturnStatement ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as ReturnStatement)
                .Expect("Failed to cast existing AST node to ReturnStatement.", j);
        }
        
        ReturnStatement node = new()
        {
            Id = id,
            Expression = Expression.ParseFromJ(
                j.GetChildren()
                    .FirstOrDefault()
                    .Expect("Cannot find the value of the return statement", j),
                astNodeDict
            )
        };
        
        astNodeDict[id] = node;
        return node;
    }

    public override GraphNode AddToGraph(AdjacencyGraph<GraphNode, GraphEdge> graph,
        Dictionary<string, GraphNode> astNodeDict)
    {
        if (astNodeDict.TryGetValue(Id, out var existingNode))
        {
            return existingNode;
        }
        
        GraphNode node = new()
        {
            Id = Id,
            Label = "[Return]"
        };
        graph.AddVertex(node);

        var exprNode = Expression.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new(node, exprNode));

        astNodeDict[Id] = node;
        return node;
    }
}