using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Expression("CStyleCastExpr")]
public class CStyleCastExpression : Expression
{
    public required Expression Inner { get; set; }
    
    public new static CStyleCastExpression ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as CStyleCastExpression)
                .Expect("Failed to cast existing AST node to CStyleCastExpression.", j);
        }

        var children = j.GetChildren();
        CStyleCastExpression node = new()
        {
            Id = id,
            Type = j.GetTypeName(),
            ValueCategory = j.GetExpressionValueCategory(),
            Inner = Expression.ParseFromJ(
                children.FirstOrDefault()
                    .Expect("Cannot find the inner expression of the C-style cast.", j),
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
            Label = $"[CStyleCast]\n({Type})"
        };
        graph.AddVertex(node);
        
        var innerNode = Inner.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new(node, innerNode));
        
        astNodeDict[Id] = node;
        return node;
    }
}