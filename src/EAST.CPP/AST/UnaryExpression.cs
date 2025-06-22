using EAST.CPP.AST.Attributes;
using EAST.CPP.AST.Enums;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Expression("UnaryOperator")]
public class UnaryExpression : Expression
{
    public required UnaryOperation Operation { get; set; }
    public required Expression Operand { get; set; }
    public required bool IsPostfix { get; set; }
    
    public new static UnaryExpression ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as UnaryExpression)
                .Expect("Failed to cast existing AST node to UnaryExpression.", j);
        }
        
        var children = j.GetChildren();
        UnaryExpression node = new()
        {
            Id = id,
            Type = j.GetTypeName(),
            ValueCategory = j.GetExpressionValueCategory(),
            IsPostfix = j.Value<bool>("isPostfix")
                .Expect("Cannot find 'isPostfix' in the unary expression", j),
            Operation = UnaryOperation.FromString(
                j.Value<string>("opcode")
                    .Expect("Cannot find operation code of the unary expression", j)
            ),
            Operand = Expression.ParseFromJ(
                children.FirstOrDefault()
                    .Expect("Cannot find operand of the unary expression", j),
                astNodeDict
            )
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
            Label = Operation + (IsPostfix ? " (postfix)" : " (prefix)")
        };
        graph.AddVertex(node);
        
        var operandNode = Operand.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new(node, operandNode));
        
        astNodeDict[Id] = node;
        return node;
    }
}