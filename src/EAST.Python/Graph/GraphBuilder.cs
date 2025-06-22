using Newtonsoft.Json.Linq;
using QuikGraph;
using QuikGraph.Graphviz;
using QuikGraph.Graphviz.Dot;

namespace EAST.Python.Graph;

public class GraphBuilder
{
    // "Attribute"
    
    private static Dictionary<string, Func<JObject, string>> LeafKeys = new()
    {
        ["Name"] = j => j.Value<string>("id")!,
        ["Constant"] = j => j.Value<string>("value")!,
        ["Break"] = _ => "Break",
        ["Eq"] = _ => "==",
        ["And"] = _ => "&&",
        ["Not"] = _ => "!"
    };
    
    private static Dictionary<string, string[]> ChildrenKeys = new()
    {
        ["Module"] = ["body"],
        ["Assign"] = ["targets", "value"],
        ["Call"] = ["func", "args"],
        ["For"] = ["target", "iter", "body"],
        ["If"] = ["test", "body", "orelse"],
        ["Compare"] = ["left", "ops", "comparators"],
        ["Expr"] = ["value"],
        ["BoolOp"] = ["op", "values"],
        ["UnaryOp"] = ["op", "operand"],
        ["List"] = ["elts"],
        ["Attribute"] = ["value", "attr"]
    };

    public static void BuildGraph(JObject root, string outputFilePath)
    {
        // Initialize the graph
        var graph = new AdjacencyGraph<Node, Edge<Node>>();

        // Parse the root node
        AddToGraph(root, graph);

        // Write the graph to a file
        GraphvizAlgorithm<Node, Edge<Node>> viz = new(graph);
        viz.FormatVertex += (_, args) =>
        {
            args.VertexFormat.Shape = args.Vertex.Shape;
            args.VertexFormat.Label = args.Vertex.Label;
        };
        var dot = viz.Generate();
        File.WriteAllText(outputFilePath, dot);
    }

    private static Node AddToGraph(JObject j, AdjacencyGraph<Node, Edge<Node>> graph)
    {
        var type = j.Value<string>("_type")!;
        if (LeafKeys.TryGetValue(type, out var label))
        {
            Node node = new()
            {
                Label = label(j),
                Shape = GraphvizVertexShape.Circle
            };
            graph.AddVertex(node);
            return node;
        }
        if (ChildrenKeys.TryGetValue(type, out var childrenKeys))
        {
            Node node = new()
            {
                Label = type,
                Shape = GraphvizVertexShape.Box
            };
            graph.AddVertex(node);
            foreach (var key in childrenKeys)
            {
                switch (j[key])
                {
                    case JArray {Count: >1} array:
                        Node propNode = new()
                        {
                            Label = key,
                            Shape = GraphvizVertexShape.Triangle
                        };
                        graph.AddVertex(propNode);
                        graph.AddEdge(new Edge<Node>(node, propNode));
                        foreach (var child in array.OfType<JObject>())
                        {
                            var cn = AddToGraph(child, graph);
                            graph.AddEdge(new Edge<Node>(propNode, cn));
                        }
                        break;
                    case JArray {Count: 1} array:
                        foreach (var child in array.OfType<JObject>())
                        {
                            var cn = AddToGraph(child, graph);
                            graph.AddEdge(new Edge<Node>(node, cn));
                        }
                        break;
                    case JObject child:
                        var cn2 = AddToGraph(child, graph);
                        graph.AddEdge(new Edge<Node>(node, cn2));
                        break;
                    case JValue value:
                        Node vn = new()
                        {
                            Label = value.ToString() ?? "",
                            Shape = GraphvizVertexShape.Circle
                        };
                        graph.AddVertex(vn);
                        graph.AddEdge(new Edge<Node>(node, vn));
                        break;
                    default:
                        break;
                }
            }
            return node;
        }
        throw new NotSupportedException($"Unknown type {type}");
    }
}