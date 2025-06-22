using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Expression("CXXStdInitializerListExpr")]
public class StandardInitializerListExpression : Expression
{
    public required Expression Sub { get; set; }
    
    public new static StandardInitializerListExpression ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as StandardInitializerListExpression)
                .Expect("Failed to cast existing AST node to StandardInitializerListExpression.", j);
        }

        var children = j.GetChildren();
        StandardInitializerListExpression node = new()
        {
            Id = id,
            Type = j.GetTypeName(),
            ValueCategory = j.GetExpressionValueCategory(),
            Sub = Expression.ParseFromJ(
                children.FirstOrDefault()
                    .Expect("Cannot find the sub expression of the standard initializer list.", j),
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
            Label = $"[StdInitializerList]\n({Type})"
        };
        graph.AddVertex(node);
        
        var subNode = Sub.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new(node, subNode));
        
        astNodeDict[Id] = node;
        return node;
    }
}