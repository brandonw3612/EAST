using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Expression("MaterializeTemporaryExpr")]
public class MaterializeTemporaryExpression : Expression
{
    public required Expression Inner { get; set; }
    
    public new static MaterializeTemporaryExpression ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as MaterializeTemporaryExpression)
                .Expect("Failed to cast existing AST node to MaterializeTemporaryExpression.", j);
        }
        
        MaterializeTemporaryExpression node = new()
        {
            Id = id,
            Type = j.GetTypeName(),
            ValueCategory = j.GetExpressionValueCategory(),
            Inner = Expression.ParseFromJ(
                j.GetChildren()
                    .FirstOrDefault()
                    .Expect("Cannot find inner expression in MaterializeTemporaryExpression.", j),
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
        
        var node = new GraphNode
        {
            Id = Id,
            Label = $"[MaterializeTemporary]\n({Type})"
        };
        graph.AddVertex(node);
        
        var innerNode = Inner.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new GraphEdge(node, innerNode));
        
        astNodeDict[Id] = node;
        return node;
    }
}