using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Expression("CXXMemberCallExpr")]
public class MemberCallExpression : Expression
{
    public required Expression ObjectExpression { get; set; }
    public required string FunctionName { get; set; }
    public required Expression[] Arguments { get; set; }
    public required bool IsArrow { get; set; }
    
    public new static MemberCallExpression ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as MemberCallExpression)
                .Expect("Failed to cast existing AST node to MemberCallExpression.", j);
        }
        
        var children = j.GetChildren();
        var memberExpr = children.FirstOrDefault()
            .Expect("Cannot find member expression in member call expression.", j);
        MemberCallExpression node = new()
        {
            Id = id,
            Type = j.GetTypeName(),
            ValueCategory = j.GetExpressionValueCategory(),
            FunctionName = memberExpr.Value<string>("name")
                .Expect("Cannot find member name in member call expression.", j),
            IsArrow = memberExpr.Value<bool>("isArrow")
                .Expect("Cannot find 'isArrow' in member call expression.", j),
            ObjectExpression = Expression.ParseFromJ(
                memberExpr.GetChildren()
                    .FirstOrDefault()
                    .Expect("Cannot find object expression in member call expression.", j),
                astNodeDict
            ),
            Arguments = children.Skip(1)
                .Select(jj => Expression.ParseFromJ(jj, astNodeDict))
                .ToArray()
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
            Label = $"[MemberCall]\n{FunctionName}()"
        };
        graph.AddVertex(node);

        var objNode = ObjectExpression.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new(node, objNode)
        {
            Label = "Obj"
        });
        
        foreach (var arg in Arguments)
        {
            var argNode = arg.AddToGraph(graph, astNodeDict);
            graph.AddEdge(new(node, argNode)
            {
                Label = "Arg"
            });
        }

        astNodeDict[Id] = node;
        return node;
    }
}