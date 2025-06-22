using QuikGraph.Graphviz.Dot;

namespace EAST.Python.Graph;

public record Node
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Label { get; set; } = string.Empty;
    public GraphvizVertexShape Shape { get; set; }
}