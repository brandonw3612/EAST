using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;
using QuikGraph.Graphviz.Dot;

namespace EAST.CPP.AST;

[Statement("CompoundStmt")]
public class CompoundStatement : Statement
{
    public required List<Statement> Statements { get; set; } = new();
    
    public new static CompoundStatement ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as CompoundStatement)
                .Expect("Failed to cast existing AST node to CompoundStatement.", j);
        }
        
        var children = j.GetChildren();
        CompoundStatement node =  new()
        {
            Id = id,
            Statements = children.Select(jj => Statement.ParseFromJ(jj, astNodeDict))
                .ToList()
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
            Label = "[Compound]"
        };
        graph.AddVertex(node);
        
        List<GraphNode> statementNodes = new();
        for (var i = 0; i < Statements.Count; i++)
        {
            var statement = Statements[i];
            var statementNode = statement.AddToGraph(graph, astNodeDict);
            graph.AddEdge(new(node, statementNode));
            
            statementNodes.Add(statementNode);
            if (i > 0)
            {
                graph.AddEdge(new GraphEdge(statementNodes[i - 1], statementNode)
                {
                    Style = GraphvizEdgeStyle.Dashed
                });
            }
        }
        
        astNodeDict[Id] = node;
        return node;
    }
}