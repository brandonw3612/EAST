using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Statement("IfStmt")]
public class IfStatement : Statement
{
    public required Expression? Condition { get; set; }
    public required DeclarationStatement? ConditionVariableDeclaration { get; set; }
    public required Statement Then { get; set; }
    public required Statement? Else { get; set; }
    
    public new static IfStatement ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as IfStatement)
                .Expect("Failed to cast existing AST node to IfStatement.", j);
        }
        
        var children = j.GetChildren();

        Expression? condition;
        try
        {
            condition = Expression.ParseFromJ(
                children.FirstOrDefault()
                    .Expect("Cannot find condition in IfStatement.", j),
                astNodeDict
            );
        }
        catch
        {
            condition = null;
        }
        
        DeclarationStatement? declaration;
        try
        {
            declaration = DeclarationStatement.ParseFromJ(
                children.FirstOrDefault()
                    .Expect("Cannot find condition variable declaration in IfStatement.", j),
                astNodeDict
            );
        }
        catch
        {
            declaration = null;
        }
        
        if (condition is null && declaration is null)
        {
            throw new NotSupportedException("IfStatement must have either a condition or a condition variable declaration.");
        }
        
        IfStatement node = new()
        {
            Id = id,
            Condition = condition,
            ConditionVariableDeclaration = declaration,
            Then = Statement.ParseFromJ(
                children.ElementAtOrDefault(1)
                    .Expect("Cannot find then statement in IfStatement.", j),
                astNodeDict
            ),
            Else = children.Length > 2 ? Statement.ParseFromJ(
                children.ElementAtOrDefault(2)
                    .Expect("Cannot find else statement in IfStatement.", j),
                astNodeDict
            ) : null
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
            Label = "[If]"
        };
        graph.AddVertex(node);

        if (Condition != null)
        {
            var conditionNode = Condition.AddToGraph(graph, astNodeDict);
            graph.AddEdge(new GraphEdge(node, conditionNode)
            {
                Label = "Cond"
            });
        }

        if (ConditionVariableDeclaration != null)
        {
            var declarationNode = ConditionVariableDeclaration.AddToGraph(graph, astNodeDict);
            graph.AddEdge(new GraphEdge(node, declarationNode)
            {
                Label = "Cond"
            });
        }

        var thenNode = Then.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new (node, thenNode)
        {
            Label = "T"
        });

        if (Else != null)
        {
            var elseNode = Else.AddToGraph(graph, astNodeDict);
            graph.AddEdge(new(node, elseNode)
            {
                Label = "F"
            });
        }

        astNodeDict[Id] = node;
        return node;
    }
}