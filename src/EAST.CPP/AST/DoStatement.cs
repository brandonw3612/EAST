using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using QuikGraph;

namespace EAST.CPP.AST;

[Statement("DoStmt")]
public class DoStatement : Statement
{
    public required Expression Condition { get; set; }
    public required Statement Body { get; set; }
    
    public new static DoStatement ParseFromJ(Newtonsoft.Json.Linq.JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as DoStatement)
                .Expect("Failed to cast existing AST node to DoStatement.", j);
        }

        var children = j.GetChildren();
        DoStatement node = new()
        {
            Id = id,
            Body = Statement.ParseFromJ(
                children.FirstOrDefault()
                    .Expect("Cannot find the body of the do statement.", j),
                astNodeDict
            ),
            Condition = Expression.ParseFromJ(
                children.ElementAtOrDefault(1)
                    .Expect("Cannot find the condition of the do statement.", j),
                astNodeDict
            )
        };

        astNodeDict[id] = node;
        return node;
    }
    
    public override GraphNode AddToGraph(AdjacencyGraph<GraphNode, GraphEdge> graph, Dictionary<string, GraphNode> astNodeDict)
    {
        if (astNodeDict.TryGetValue(Id, out var existing))
        {
            return existing;
        }

        GraphNode node = new()
        {
            Id = Id,
            Label = "[Do]"
        };
        graph.AddVertex(node);
        
        var bodyNode = Body.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new(node, bodyNode)
        {
            Label = "Body"
        });
        
        var conditionNode = Condition.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new(node, conditionNode)
        {
            Label = "Cond"
        });
        
        astNodeDict[Id] = node;
        return node;
    }
}