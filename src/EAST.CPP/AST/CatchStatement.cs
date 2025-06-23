using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Statement("CXXCatchStmt")]
public class CatchStatement : Statement
{
    public required VariableDeclaration? ExceptionDeclaration { get; set; }
    public required Statement Block { get; set; }
    
    public new static CatchStatement ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as CatchStatement)
                .Expect("Failed to cast existing AST node to CatchStatement.", j);
        }

        var children = j.GetChildren();

        VariableDeclaration? varDecl;
        try
        {
            varDecl = VariableDeclaration.ParseFromJ(
                children.FirstOrDefault()
                    .Expect("Cannot find exception declaration in CatchStatement.", j),
                astNodeDict
            );
        }
        catch
        {
            varDecl = null;
        }
        
        CatchStatement node = new()
        {
            Id = id,
            ExceptionDeclaration = varDecl,
            Block = Statement.ParseFromJ(
                children.Skip(1).FirstOrDefault()
                    .Expect("Cannot find block in CatchStatement.", j),
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
            Label = "[Catch]"
        };
        graph.AddVertex(node);

        if (ExceptionDeclaration != null)
        {
            var exceptionNode = ExceptionDeclaration.AddToGraph(graph, astNodeDict);
            graph.AddEdge(new GraphEdge(node, exceptionNode)
            {
                Label = "Excp"
            });
        }

        var blockNode = Block.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new GraphEdge(node, blockNode)
        {
            Label = "Block"
        });

        astNodeDict[Id] = node;
        return node;
    }
}