using System.Text.Json.Serialization;
using EAST.CPP.AST;
using EAST.CPP.AST.Enums;
using Newtonsoft.Json.Linq;

namespace EAST.CPP.Extensions;

public static class JObjectExtensions
{
    public static bool HasChildren(this JObject j) => j.Value<JArray>("inner")?.Count > 0;

    public static string GetId(this JObject j) =>
        j.Value<string>("id")
            .Expect("Cannot find the ID of the node.", j);
    
    public static JObject[] GetNullableChildren(this JObject j) => 
        j.Value<JArray>("inner") is { } children 
            ? children.OfType<JObject>().ToArray() 
            : [];

    public static JObject[] GetChildren(this JObject j) =>
        j.Value<JArray>("inner")
            .Expect("Cannot find children objects under 'inner' key.", j)
            .OfType<JObject>()
            .ToArray();

    public static string GetTypeName(this JObject j) =>
        TypeDeclaration.ParseFromJ(
            j.Value<JObject>("type")
                .Expect("Cannot find the type of the expression.", j)
        ).TypeName;
    
    public static ValueCategory GetExpressionValueCategory(this JObject j) =>
        ValueCategory.FromString(
            j.Value<string>("valueCategory")
                .Expect("Cannot find the value category of the expression.", j)
        );
}