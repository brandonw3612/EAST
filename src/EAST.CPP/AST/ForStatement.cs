using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Statement("ForStmt")]
public class ForStatement : Statement
{
    // Stmt *Init, Expr *Cond, VarDecl *condVar, Expr *Inc, Stmt *Body
    public required Statement Initialization { get; set; }
    public required Expression Condition { get; set; }
    public required Expression Increment { get; set; }
    public required Statement Body { get; set; }
    
    public new static ForStatement ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as ForStatement)
                .Expect("Failed to cast existing AST node to ForStatement.", j);
        }
        
        var children = j.GetChildren();
        ForStatement node = new()
        {
            Id = id,
            Initialization = Statement.ParseFromJ(
                children.FirstOrDefault()
                    .Expect("Cannot find initialization statement in for statement.", j),
                astNodeDict
            ),
            Condition = Expression.ParseFromJ(
                children.ElementAtOrDefault(2)
                    .Expect("Cannot find condition expression in for statement.", j),
                astNodeDict
            ),
            Increment = Expression.ParseFromJ(
                children.ElementAtOrDefault(3)
                    .Expect("Cannot find increment expression in for statement.", j),
                astNodeDict
            ),
            Body = Statement.ParseFromJ(
                children.ElementAtOrDefault(4)
                    .Expect("Cannot find body statement in for statement.", j),
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
            Label = "[For]"
        };
        graph.AddVertex(node);
        
        var initNode = Initialization.AddToGraph(graph, astNodeDict);
        var condNode = Condition.AddToGraph(graph, astNodeDict);
        var incNode = Increment.AddToGraph(graph, astNodeDict);
        var bodyNode = Body.AddToGraph(graph, astNodeDict);
        
        graph.AddEdge(new(node, initNode)
        {
            Label = "Init"
        });
        graph.AddEdge(new(node, condNode)
        {
            Label = "Cond"
        });
        graph.AddEdge(new(node, incNode)
        {
            Label = "Inc"
        });
        graph.AddEdge(new(node, bodyNode)
        {
            Label = "Body"
        });

        astNodeDict[Id] = node;
        return node;
    }
}