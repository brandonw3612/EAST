using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Statement("SwitchStmt")]
public class SwitchStatement : Statement
{
    public required Expression Condition { get; set; }
    public required Statement Cases { get; set; }
    
    public new static SwitchStatement ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as SwitchStatement)
                .Expect("Failed to cast existing AST node to SwitchStatement.", j);
        }

        var children = j.GetChildren();
        SwitchStatement node = new()
        {
            Id = id,
            Condition = Expression.ParseFromJ(
                children.FirstOrDefault()
                    .Expect("Cannot find the condition of the switch statement.", j),
                astNodeDict
            ),
            Cases = Statement.ParseFromJ(
                children.ElementAtOrDefault(1)
                    .Expect("Cannot find the cases of the switch statement.", j),
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
            Label = "[Switch]"
        };
        graph.AddVertex(node);
        
        var conditionNode = Condition.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new(node, conditionNode)
        {
            Label = "Cond"
        });
        
        var casesNode = Cases.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new(node, casesNode)
        {
            Label = "Cases"
        });
        
        astNodeDict[Id] = node;
        return node;
    }
}