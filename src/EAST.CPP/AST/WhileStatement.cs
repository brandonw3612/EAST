using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Statement("WhileStmt")]
public class WhileStatement : Statement
{
    public required Expression Condition { get; set; }
    public required Statement Body { get; set; }
    
    public new static WhileStatement ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as WhileStatement)
                .Expect("Failed to cast existing AST node to WhileStatement.", j);
        }

        var children = j.GetChildren();
        WhileStatement node = new()
        {
            Id = id,
            Condition = Expression.ParseFromJ(
                children.FirstOrDefault()
                    .Expect("Cannot find the condition of the while statement.", j),
                astNodeDict
            ),
            Body = Statement.ParseFromJ(
                children.ElementAtOrDefault(1)
                    .Expect("Cannot find the body of the while statement.", j),
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
            Label = "[While]"
        };
        graph.AddVertex(node);
        
        var conditionNode = Condition.AddToGraph(graph, astNodeDict);
        var bodyNode = Body.AddToGraph(graph, astNodeDict);
        
        graph.AddEdge(new GraphEdge(node, conditionNode)
        {
            Label = "Cond"
        });
        graph.AddEdge(new GraphEdge(node, bodyNode)
        {
            Label = "Body"
        });
        
        astNodeDict[Id] = node;
        return node;
    }
}