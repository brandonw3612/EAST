using System.Net.NetworkInformation;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

public class ProgramDeclaration : IGraphNode
{
    public required string Id { get; set; }
    public List<FunctionDeclaration> Functions { get; set; } = new();

    public static ProgramDeclaration ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as ProgramDeclaration)
                .Expect("Failed to cast existing AST node to ProgramDeclaration.", j);
        }
        
        var children = j.GetChildren();
        List<FunctionDeclaration> functions = new();
        foreach (var child in children)
        {
            switch (child.Value<string>("kind"))
            {
                case "TypedefDecl":
                case "NamespaceDecl":
                case "CXXRecordDecl":
                case "LinkageSpecDecl":
                case "EnumDecl":
                case "BuiltinTemplateDecl":
                case "VarDecl":
                case "UsingDirectiveDecl": break;
                case "FunctionDecl":
                {
                    functions.Add(FunctionDeclaration.ParseFromJ(child, astNodeDict));
                    break;
                }
                default: throw new NotSupportedException("Unhandled top-level kind: " + child.Value<string>("kind"));
            }
        }
        return new()
        {
            Id = id,
            Functions = functions
        };
    }


    public GraphNode AddToGraph(AdjacencyGraph<GraphNode, GraphEdge> graph, Dictionary<string, GraphNode> astNodeDict)
    {
        GraphNode node = new()
        {
            Id = Id,
            Label = "[Program]"
        };
        graph.AddVertex(node);
        
        foreach (var function in Functions)
        {
            var functionNode = function.AddToGraph(graph, astNodeDict);
            graph.AddEdge(new(node, functionNode));
        }
        
        astNodeDict[Id] = node;
        return node;
    }
}