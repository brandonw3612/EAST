using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Expression("ParenExpr")]
public class ParenthesizedExpression : Expression
{
    public required Expression Inner { get; set; }
    
    public new static ParenthesizedExpression ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as ParenthesizedExpression)
                .Expect("Failed to cast existing AST node to ParenthesizedExpression.", j);
        }
        
        var children = j.GetChildren();
        ParenthesizedExpression node = new()
        {
            Id = id,
            Type = j.GetTypeName(),
            ValueCategory = j.GetExpressionValueCategory(),
            Inner = Expression.ParseFromJ(
                children.FirstOrDefault()
                    .Expect("Cannot find inner expression in parenthesized expression.", j),
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
            Label = "[Parenthesized]"
        };
        graph.AddVertex(node);
        
        var innerNode = Inner.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new(node, innerNode));
        
        astNodeDict[Id] = innerNode;
        return node;
    }
}