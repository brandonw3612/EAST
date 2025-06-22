using EAST.CPP.AST.Attributes;
using EAST.CPP.AST.Enums;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Expression("CallExpr")]
public class CallExpression : Expression
{
    public required Expression Callee { get; set; }
    public required List<Expression> Arguments { get; set; } = new();
    
    public new static CallExpression ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as CallExpression)
                .Expect("Failed to cast existing AST node to CallExpression.", j);
        }
        
        var children = j.GetChildren();
        CallExpression node = new()
        {
            Id = id,
            Type = j.GetTypeName(),
            ValueCategory = j.GetExpressionValueCategory(),
            Callee = Expression.ParseFromJ(
                children.ElementAtOrDefault(0)
                    .Expect("Cannot find callee of the call expression", j),
                astNodeDict
            ),
            Arguments = children.Skip(1)
                .Select(jj => Expression.ParseFromJ(jj, astNodeDict))
                .ToList()
        };
        
        astNodeDict[id] = node;
        return node;
    }

    public override GraphNode AddToGraph(AdjacencyGraph<GraphNode, GraphEdge> graph,
        Dictionary<string, GraphNode> astNodeDict)
    {
        if (astNodeDict.TryGetValue(Id, out var existing))
        {
            return existing;
        }

        GraphNode node = new()
        {
            Id = Id,
            Label = "[Call]"
        };
        graph.AddVertex(node);
        
        var calleeNode = Callee.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new(node, calleeNode)
        {
            Label = "Callee"
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