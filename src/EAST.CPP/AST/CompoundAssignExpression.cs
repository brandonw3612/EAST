using EAST.CPP.AST.Attributes;
using EAST.CPP.AST.Enums;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Expression("CompoundAssignOperator")]
public class CompoundAssignExpression : Expression
{
    public required Expression LeftExpression { get; set; }
    public required Expression RightExpression { get; set; }
    public required BinaryOperation Operation { get; set; }
    
    public new static CompoundAssignExpression ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as CompoundAssignExpression)
                .Expect("Failed to cast existing AST node to CompoundAssignExpression.", j);
        }

        var children = j.GetNullableChildren();
        CompoundAssignExpression node = new()
        {
            Id = id,
            LeftExpression = Expression.ParseFromJ(
                children.FirstOrDefault()
                    .Expect("Cannot find left expression in CompoundAssignExpression.", j),
                astNodeDict
            ),
            RightExpression = Expression.ParseFromJ(
                children.ElementAtOrDefault(1)
                    .Expect("Cannot find right expression in CompoundAssignExpression.", j),
                astNodeDict
            ),
            Operation = BinaryOperation.FromString(
                j.Value<string>("opcode")
                    .Expect("Cannot find the operation code in the compound assignment expression.", j)
            ),
            Type = j.GetTypeName(),
            ValueCategory = j.GetExpressionValueCategory()
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
            Label = Operation.ToString()
        };
        graph.AddVertex(node);

        var leftNode = LeftExpression.AddToGraph(graph, astNodeDict);
        var rightNode = RightExpression.AddToGraph(graph, astNodeDict);
        
        graph.AddEdge(new GraphEdge(leftNode, node));
        graph.AddEdge(new GraphEdge(rightNode, node));

        astNodeDict[Id] = node;
        return node;
    }
}