using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Expression("CXXOperatorCallExpr")]
public class OperatorCallExpression : Expression
{
    public required Expression Function { get; set; }
    public required Expression[] Arguments { get; set; }
    
    public new static OperatorCallExpression ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as OperatorCallExpression)
                .Expect("Failed to cast existing AST node to OperatorCallExpression.", j);
        }
        
        var children = j.GetChildren();
        OperatorCallExpression node = new()
        {
            Id = id,
            Type = j.GetTypeName(),
            ValueCategory = j.GetExpressionValueCategory(),
            Function = Expression.ParseFromJ(
                children.FirstOrDefault()
                    .Expect("Cannot find function in operator call expression.", j),
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
            Label = "[OperatorCall]"
        };
        graph.AddVertex(node);
        
        var functionNode = Function.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new(node, functionNode)
        {
            Label = "Func"
        });
        
        foreach (var arg in Arguments)
        {
            var argNode = arg.AddToGraph(graph, astNodeDict);
            graph.AddEdge(new (node, argNode)
            {
                Label = "Arg"
            });
        }
        
        astNodeDict[Id] = node;
        return node;
    }
}