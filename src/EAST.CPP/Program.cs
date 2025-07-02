using EAST.CPP.AST;
using EAST.CPP.Graph;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

var inputFile = args[0];
using var reader = File.OpenText(inputFile);
JsonSerializer serializer = new JsonSerializer
{
    MaxDepth = 128
};
var j = serializer.Deserialize<JObject>(new JsonTextReader(reader));
j["inner"] = new JArray(j["inner"]?.Where(j => j.Value<string>("kind") == "FunctionDecl" && j.Value<string>("name") == "main"));

Dictionary<string, object> astNodeDict = [];
var p = ProgramDeclaration.ParseFromJ(j, astNodeDict);

var outputGraph = args[1];
GraphBuilder.BuildGraph(j, outputGraph);

var trimmedGraph = args[2];
GraphBuilder.BuildGraph(p, astNodeDict, trimmedGraph);