using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Expression("InitListExpr")]
public class InitializerListExpression : Expression
{
    public required Expression[] Elements { get; set; }
    
    public new static InitializerListExpression ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as InitializerListExpression)
                .Expect("Failed to cast existing AST node to InitializerListExpression.", j);
        }

        var children = j.GetChildren();
        InitializerListExpression node = new()
        {
            Id = id,
            Type = j.GetTypeName(),
            ValueCategory = j.GetExpressionValueCategory(),
            Elements = children.Select(jj => Expression.ParseFromJ(jj, astNodeDict)).ToArray()
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
            Label = $"[InitList]\n({Type})"
        };
        graph.AddVertex(node);
        
        foreach (var element in Elements)
        {
            var elementNode = element.AddToGraph(graph, astNodeDict);
            graph.AddEdge(new(node, elementNode));
        }
        
        astNodeDict[Id] = node;
        return node;
    }
}