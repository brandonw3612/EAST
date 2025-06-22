using EAST.Python.Graph;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

var name = "task2";
var astPath = $"/Users/brandon/Developer/EAST/samples/{name}/{name}.ast.json";
var astGraphDotPath = $"/Users/brandon/Developer/EAST/samples/{name}/{name}.ast.dot";

var json = File.ReadAllText(astPath);
var j = JsonConvert.DeserializeObject<JObject>(json, new JsonSerializerSettings
{
    MaxDepth = 128
});

GraphBuilder.BuildGraph(j, astGraphDotPath);