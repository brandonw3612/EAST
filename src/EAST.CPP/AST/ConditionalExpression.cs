using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Expression("ConditionalOperator")]
public class ConditionalExpression : Expression
{
    public required Expression Condition { get; set; }
    public required Expression True { get; set; }
    public required Expression False { get; set; }
    
    public new static ConditionalExpression ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as ConditionalExpression)
                .Expect("Failed to cast existing AST node to ConditionalExpression.", j);
        }
        
        var children = j.GetChildren();
        ConditionalExpression node = new()
        {
            Id = id,
            Type = j.GetTypeName(),
            ValueCategory = j.GetExpressionValueCategory(),
            Condition = Expression.ParseFromJ(
                children.FirstOrDefault()
                    .Expect("Cannot find condition in conditional expression.", j),
                astNodeDict
            ),
            True = Expression.ParseFromJ(
                children.ElementAtOrDefault(1)
                    .Expect("Cannot find true expression in conditional expression.", j),
                astNodeDict
            ),
            False = Expression.ParseFromJ(
                children.ElementAtOrDefault(2)
                    .Expect("Cannot find false expression in conditional expression.", j),
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
            Label = "[Conditional]"
        };
        graph.AddVertex(node);
        
        var conditionNode = Condition.AddToGraph(graph, astNodeDict);
        var trueNode = True.AddToGraph(graph, astNodeDict);
        var falseNode = False.AddToGraph(graph, astNodeDict);

        graph.AddEdge(new(node, conditionNode)
        {
            Label = "Cond"
        });
        graph.AddEdge(new(node, trueNode)
        {
            Label = "T"
        });
        graph.AddEdge(new(node, falseNode)
        {
            Label = "F"
        });
        
        astNodeDict[Id] = node;
        return node;
    }
}