using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using QuikGraph;

namespace EAST.CPP.AST;

[Expression("ConstantExpr")]
public class ConstantExpression : Expression
{
    public required Expression Inner { get; set; }
    
    public new static ConstantExpression ParseFromJ(Newtonsoft.Json.Linq.JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as ConstantExpression)
                .Expect("Failed to cast existing AST node to ConstantExpression.", j);
        }

        var children = j.GetChildren();
        ConstantExpression node = new()
        {
            Id = id,
            Type = j.GetTypeName(),
            ValueCategory = j.GetExpressionValueCategory(),
            Inner = Expression.ParseFromJ(
                children.FirstOrDefault()
                    .Expect("Cannot find the inner expression of the constant expression.", j),
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
            Label = $"[Constant]\n({Type})"
        };
        graph.AddVertex(node);
        
        var innerNode = Inner.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new(node, innerNode));
        
        astNodeDict[Id] = node;
        return node;
    }
}