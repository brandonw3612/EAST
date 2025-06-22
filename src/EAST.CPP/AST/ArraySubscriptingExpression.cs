using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Expression("ArraySubscriptExpr")]
public class ArraySubscriptingExpression : Expression
{
    public required Expression Left { get; set; }
    public required Expression Right { get; set; }
    
    public new static ArraySubscriptingExpression ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as ArraySubscriptingExpression)
                .Expect("Failed to cast existing AST node to ArraySubscriptingExpression.", j);
        }

        var children = j.GetChildren();
        ArraySubscriptingExpression node = new()
        {
            Id = id,
            Type = j.GetTypeName(),
            ValueCategory = j.GetExpressionValueCategory(),
            Left = Expression.ParseFromJ(
                children.FirstOrDefault()
                    .Expect("Cannot find left expression in the array subscripting expression.", j),
                astNodeDict
            ),
            Right = Expression.ParseFromJ(
                children.ElementAtOrDefault(1)
                    .Expect("Cannot find right expression in the array subscripting expression.", j),
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
            Label = $"[ArraySubscripting]\n({Type})"
        };
        graph.AddVertex(node);

        var leftNode = Left.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new(node, leftNode));

        var rightNode = Right.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new(node, rightNode));

        astNodeDict[Id] = node;
        return node;
    }
}