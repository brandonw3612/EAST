using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Expression("ExprWithCleanups")]
public class ExpressionWithCleanups : Expression
{
    public required Expression InnerExpression { get; set; }
    
    public new static ExpressionWithCleanups ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as ExpressionWithCleanups)
                .Expect("Failed to cast existing AST node to ExpressionWithCleanups.", j);
        }
        
        ExpressionWithCleanups node = new()
        {
            Id = id,
            Type = j.GetTypeName(),
            ValueCategory = j.GetExpressionValueCategory(),
            InnerExpression = Expression.ParseFromJ(
                j.GetChildren()
                    .FirstOrDefault()
                    .Expect("Cannot find inner expression in ExprWithCleanups.", j),
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
            Label = $"[WithCleanups]\n({Type})"
        };
        graph.AddVertex(node);
        
        var innerNode = InnerExpression.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new(node, innerNode));
        
        astNodeDict[Id] = node;
        return node;
    }
}