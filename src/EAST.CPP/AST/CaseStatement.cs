using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using QuikGraph;

namespace EAST.CPP.AST;

[Statement("CaseStmt")]
public class CaseStatement : Statement
{
    public required Expression Left { get; set; }
    public required Expression Right { get; set; }
    
    public new static CaseStatement ParseFromJ(Newtonsoft.Json.Linq.JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as CaseStatement)
                .Expect("Failed to cast existing AST node to CaseStatement.", j);
        }

        var children = j.GetChildren();
        CaseStatement node = new()
        {
            Id = id,
            Left = Expression.ParseFromJ(
                children.FirstOrDefault()
                    .Expect("Cannot find the left expression of the case statement.", j),
                astNodeDict
            ),
            Right = Expression.ParseFromJ(
                children.ElementAtOrDefault(1)
                    .Expect("Cannot find the right expression of the case statement.", j),
                astNodeDict
            )
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
            Label = "[Case]"
        };
        graph.AddVertex(node);
        
        var leftNode = Left.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new(node, leftNode)
        {
            Label = "Left"
        });
        
        var rightNode = Right.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new(node, rightNode)
        {
            Label = "Right"
        });
        
        astNodeDict[Id] = node;
        return node;
    }
}