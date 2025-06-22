using QuikGraph.Graphviz.Dot;

namespace EAST.CPP.Graph;

public class GraphNode
{
    public required string Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public GraphvizVertexShape Shape { get; set; } = GraphvizVertexShape.Box;
}