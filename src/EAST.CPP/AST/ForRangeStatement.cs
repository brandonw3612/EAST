using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Statement("CXXForRangeStmt")]
public class ForRangeStatement : Statement
{
    public required Statement? Initializer { get; set; }
    public required DeclarationStatement Range { get; set; }
    public required DeclarationStatement BeginStatement { get; set; }
    public required DeclarationStatement EndStatement { get; set; }
    public required Expression Condition { get; set; }
    public required Expression Increment { get; set; }
    public required DeclarationStatement LoopVariable { get; set; }
    public required Statement Body { get; set; }
    
    public new static ForRangeStatement ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as ForRangeStatement)
                .Expect("Failed to cast existing AST node to ForRangeStatement.", j);
        }
        
        var children = j.GetChildren();
        ForRangeStatement node = new()
        {
            Id = id,
            Initializer = ParseNullableFromJ(
                children.FirstOrDefault()
                    .Expect("Cannot find initializer in the for-range statement.", j),
                astNodeDict
            ),
            Range = DeclarationStatement.ParseFromJ(
                children.ElementAtOrDefault(1)
                    .Expect("Cannot find range in the for-range statement.", j),
                astNodeDict
            ),
            BeginStatement = DeclarationStatement.ParseFromJ(
                children.ElementAtOrDefault(2)
                    .Expect("Cannot find begin statement in the for-range statement.", j),
                astNodeDict
            ),
            EndStatement = DeclarationStatement.ParseFromJ(
                children.ElementAtOrDefault(3)
                    .Expect("Cannot find end statement in the for-range statement.", j),
                astNodeDict
            ),
            Condition = Expression.ParseFromJ(
                children.ElementAtOrDefault(4)
                    .Expect("Cannot find condition in the for-range statement.", j),
                astNodeDict
            ),
            Increment = Expression.ParseFromJ(
                children.ElementAtOrDefault(5)
                    .Expect("Cannot find increment in the for-range statement.", j),
                astNodeDict
            ),
            LoopVariable = DeclarationStatement.ParseFromJ(
                children.ElementAtOrDefault(6)
                    .Expect("Cannot find loop variable in the for-range statement.", j),
                astNodeDict
            ),
            Body = Statement.ParseFromJ(
                children.ElementAtOrDefault(7)
                    .Expect("Cannot find body in the for-range statement.", j),
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
            Label = "[For-Range]"
        };
        graph.AddVertex(node);
        
        if (Initializer is not null)
        {
            var initNode = Initializer.AddToGraph(graph, astNodeDict);
            graph.AddEdge(new GraphEdge(node, initNode)
            {
                Label = "Init"
            });
        }
        var rangeNode = Range.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new GraphEdge(node, rangeNode)
        {
            Label = "Range"
        });
        var beginNode = BeginStatement.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new GraphEdge(node, beginNode)
        {
            Label = "Begin"
        });
        var endNode = EndStatement.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new GraphEdge(node, endNode)
        {
            Label = "End"
        });
        var conditionNode = Condition.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new GraphEdge(node, conditionNode)
        {
            Label = "Cond"
        });
        var incrementNode = Increment.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new GraphEdge(node, incrementNode)
        {
            Label = "Inc"
        });
        var loopVarNode = LoopVariable.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new GraphEdge(node, loopVarNode)
        {
            Label = "LoopVar"
        });
        var bodyNode = Body.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new GraphEdge(node, bodyNode)
        {
            Label = "Body"
        });
        
        astNodeDict[Id] = node;
        return node;
    }
}