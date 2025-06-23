using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Expression("CXXDefaultInitExpr")]
public class DefaultInitializerExpression : Expression
{
    public required Expression Inner { get; set; }
    
    public new static DefaultInitializerExpression ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as DefaultInitializerExpression)
                .Expect("Failed to cast existing AST node to DefaultInitializerExpression.", j);
        }

        var children = j.GetNullableChildren();
        DefaultInitializerExpression node = new()
        {
            Id = id,
            Type = j.GetTypeName(),
            ValueCategory = j.GetExpressionValueCategory(),
            Inner = Expression.ParseFromJ(
                children.FirstOrDefault()
                    .Expect("Cannot find inner expression in DefaultInitializerExpression.", j),
                astNodeDict
            )
        };

        astNodeDict[id] = node;
        return node;
    }
    
    public override GraphNode AddToGraph(AdjacencyGraph<GraphNode, GraphEdge> graph, Dictionary<string, GraphNode> astNodeDict)
    {
        if (astNodeDict.TryGetValue(Id, out var existingNode))
        {
            return existingNode;
        }
        
        GraphNode node = new()
        {
            Id = Id,
            Label = "[DefaultInitializer]"
        };
        graph.AddVertex(node);
        
        var innerNode = Inner.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new GraphEdge(node, innerNode));
        
        astNodeDict[Id] = node;
        return node;
    }
}