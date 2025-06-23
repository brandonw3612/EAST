using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Expression("UnaryExprOrTypeTraitExpr")]
public class UnaryExpressionOrTypeTraitExpression : Expression
{
    public required string Name { get; set; }
    public required Expression Inner { get; set; }
    
    public new static UnaryExpressionOrTypeTraitExpression ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as UnaryExpressionOrTypeTraitExpression)
                .Expect("Failed to cast existing AST node to UnaryExpressionOrTypeTraitExpression.", j);
        }

        var children = j.GetChildren();
        UnaryExpressionOrTypeTraitExpression node = new()
        {
            Id = id,
            Type = j.GetTypeName(),
            ValueCategory = j.GetExpressionValueCategory(),
            Name = j.Value<string>("name")
                .Expect("Cannot find the name of the unary expression or type trait.", j),
            Inner = Expression.ParseFromJ(
                children.FirstOrDefault()
                    .Expect("Cannot find the inner expression of the unary expression or type trait.", j),
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
            Label = $"{Name}: {Type}"
        };
        graph.AddVertex(node);
        
        var innerNode = Inner.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new(node, innerNode));
        
        astNodeDict[Id] = node;
        return node;
    }
}