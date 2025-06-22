using EAST.CPP.AST.Attributes;
using EAST.CPP.AST.Enums;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Expression("BinaryOperator")]
public class BinaryExpression : Expression
{
    public required BinaryOperation Operation { get; set; }
    public required Expression Left { get; set; }
    public required Expression Right { get; set; }

    public new static BinaryExpression ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as BinaryExpression)
                .Expect("Internal error: Cannot cast existing AST node to BinaryExpression.", j);
        }
        
        var children = j.GetChildren();
        BinaryExpression node = new()
        {
            Id = id,
            Type = j.GetTypeName(),
            ValueCategory = j.GetExpressionValueCategory(),
            Operation = BinaryOperation.FromString(
                j.Value<string>("opcode")
                    .Expect("Cannot find operation code of the binary expression", j)
            ),
            Left = Expression.ParseFromJ(
                children.ElementAtOrDefault(0)
                    .Expect("Cannot find left operand of the binary expression", j),
                astNodeDict
            ),
            Right = Expression.ParseFromJ(
                children.ElementAtOrDefault(1)
                    .Expect("Cannot find right operand of the binary expression", j),
                astNodeDict
            )
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
            Label = Operation.ToString()
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