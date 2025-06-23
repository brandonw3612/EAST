using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Expression("CXXFunctionalCastExpr")]
public class FunctionalCastExpression : Expression
{
    public required Expression Operand { get; set; }
    
    public new static FunctionalCastExpression ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as FunctionalCastExpression)
                .Expect("Failed to cast existing AST node to FunctionalCastExpression.", j);
        }
        
        var children = j.GetNullableChildren();
        FunctionalCastExpression node = new()
        {
            Id = id,
            Operand = Expression.ParseFromJ(
                children.FirstOrDefault()
                    .Expect("Cannot find operand expression in FunctionalCastExpression.", j),
                astNodeDict
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
            Label = $"[FunctionalCast]\n({Type})"
        };
        graph.AddVertex(node);
        
        var operandNode = Operand.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new(node, operandNode));
        
        astNodeDict[Id] = node;
        return node;
    }
}