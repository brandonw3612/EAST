using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Expression("ImplicitValueInitExpr")]
public class ImplicitValueInitializerExpression : Expression
{
    public new static ImplicitValueInitializerExpression ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as ImplicitValueInitializerExpression)
                .Expect("Failed to cast existing AST node to ImplicitValueInitializerExpression.", j);
        }

        ImplicitValueInitializerExpression node = new()
        {
            Id = id,
            Type = j.GetTypeName(),
            ValueCategory = j.GetExpressionValueCategory()
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
            Label = $"[ImplicitValueInit]\n({Type})"
        };
        graph.AddVertex(node);
        
        astNodeDict[Id] = node;
        return node;
    }
}