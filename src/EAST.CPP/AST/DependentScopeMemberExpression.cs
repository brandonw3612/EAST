using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using QuikGraph;

namespace EAST.CPP.AST;

[Expression("CXXDependentScopeMemberExpr")]
public class DependentScopeMemberExpression : Expression
{
    public required string MemberName { get; set; }
    public required Expression Base { get; set; }
    
    public new static DependentScopeMemberExpression ParseFromJ(Newtonsoft.Json.Linq.JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as DependentScopeMemberExpression)
                .Expect("Failed to cast existing AST node to DependentScopeMemberExpression.", j);
        }

        var children = j.GetChildren();
        DependentScopeMemberExpression node = new()
        {
            Id = id,
            Type = j.GetTypeName(),
            ValueCategory = j.GetExpressionValueCategory(),
            MemberName = j.Value<string>("member")
                .Expect("Cannot find the member name of the dependent scope member expression.", j),
            Base = Expression.ParseFromJ(
                children.FirstOrDefault()
                    .Expect("Cannot find the inner expression of the dependent scope member expression.", j),
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
            Label = $"[DepScopeMember]\n{MemberName}: {Type}"
        };
        graph.AddVertex(node);
        
        var innerNode = Base.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new(node, innerNode));
        
        astNodeDict[Id] = node;
        return node;
    }
}