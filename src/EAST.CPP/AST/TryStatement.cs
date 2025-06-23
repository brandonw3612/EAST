using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Statement("CXXTryStmt")]
public class TryStatement : Statement
{
    public required CompoundStatement Block { get; set; }
    public required Statement[] Handlers { get; set; }
    
    public new static TryStatement ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as TryStatement)
                .Expect("Failed to cast existing AST node to TryStatement.", j);
        }

        var children = j.GetChildren();
        TryStatement node = new()
        {
            Id = id,
            Block = CompoundStatement.ParseFromJ(
                children.FirstOrDefault()
                    .Expect("Cannot find block in TryStatement.", j),
                astNodeDict
            ),
            Handlers = children.Skip(1)
                .Select(jj => Statement.ParseFromJ(jj, astNodeDict))
                .ToArray()
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
            Label = "[Try]"
        };
        graph.AddVertex(node);

        var blockNode = Block.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new GraphEdge(node, blockNode)
        {
            Label = "Block"
        });

        foreach (var handler in Handlers)
        {
            var handlerNode = handler.AddToGraph(graph, astNodeDict);
            graph.AddEdge(new GraphEdge(node, handlerNode)
            {
                Label = "Hndl"
            });
        }

        astNodeDict[Id] = node;
        return node;
    }
}