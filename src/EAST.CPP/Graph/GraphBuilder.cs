using EAST.CPP.AST;
using Newtonsoft.Json.Linq;
using QuikGraph;
using QuikGraph.Graphviz;
using QuikGraph.Graphviz.Dot;

namespace EAST.CPP.Graph;

public static class GraphBuilder
{
    public static void BuildGraph(ProgramDeclaration program, Dictionary<string, object> astNodeDict, string outputFilePath)
    {
        // Initialize the graph
        var graph = new AdjacencyGraph<GraphNode, GraphEdge>();
        Dictionary<string, GraphNode> gnd = new();
        // Parse the program declaration
        program.AddToGraph(graph, gnd);
        
        // Write the graph to a file
        GraphvizAlgorithm<GraphNode, GraphEdge> viz = new(graph);
        viz.FormatVertex += (_, args) =>
        {
            args.VertexFormat.Shape = args.Vertex.Shape;
            args.VertexFormat.Label = args.Vertex.Label;
        };
        viz.FormatEdge += (_, args) =>
        {
            args.EdgeFormat.Label.Value = args.Edge.Label;
            args.EdgeFormat.Style = args.Edge.Style;
        };
        var dot = viz.Generate();
        var graphVertices = graph.Vertices.ToList();
        var appendedContent = string.Empty;
        foreach (var cs in astNodeDict.Values.OfType<CompoundStatement>().Where(static cs => cs.Statements.Count > 1))
        {
            var children = cs.Statements.Select(static s => s.Id)
                .Select(id => gnd[id])
                .Select(n => graphVertices.IndexOf(n))
                .Select(index => $"{index}; ").ToArray();
            appendedContent += "\nsubgraph {\nrank = same;\n" + string.Join("", children) + "\n}\n";
        }
        var ind = dot.LastIndexOf('}');
        dot = dot.Insert(ind, appendedContent);
        File.WriteAllText(outputFilePath, dot);
    }
    
    public static void BuildGraph(JObject root, string outputFilePath)
    {
        // Initialize the graph
        var graph = new AdjacencyGraph<JObject, Edge<JObject>>();

        // Parse the root node
        AddToGraph(graph, root);
        
        // Write the graph to a file
        GraphvizAlgorithm<JObject, Edge<JObject>> viz = new(graph);
        viz.FormatVertex += (_, args) =>
        {
            args.VertexFormat.Shape = GraphvizVertexShape.Box;
            args.VertexFormat.Label = (args.Vertex["kind"] ?? "$UnknownKind$") +
                                (args.Vertex["name"] is null ? "" : "\n" + args.Vertex["name"]);
        };
        var dot = viz.Generate();
        File.WriteAllText(outputFilePath, dot);
    }
    
    private static void AddToGraph(AdjacencyGraph<JObject, Edge<JObject>> graph, JObject j)
    {
        // Add the current node to the graph
        graph.AddVertex(j);

        // Process inner nodes if they exist
        var innerNodes = j.Value<JArray>("inner")?.OfType<JObject>();
        if (innerNodes == null) return;
        foreach (var innerNode in innerNodes)
        {
            // Recursively append inner nodes
            AddToGraph(graph, innerNode);
            // Create an edge from the current node to the inner node
            graph.AddEdge(new Edge<JObject>(j, innerNode));
        }
    }
}