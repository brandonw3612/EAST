using Newtonsoft.Json.Linq;

namespace EAST.CPP.Extensions;

public static class AssertionExtensions
{
    public static T Expect<T>(this T? value, string message, JObject j)
    {
        return value ?? throw new Exception(message + "\nObject JSON: " + j);
    }
}