using EAST.CPP.AST;

namespace EAST.CPP.External;

public static class ExternalFunctions
{
    private static Dictionary<(string Name, string Type), FunctionDeclaration> _externalFunctions = new()
    {
        [("sort", "void (std::__wrap_iter<std::pair<int, int> *>, std::__wrap_iter<std::pair<int, int> *>)")] =
            ExternalFunctionDeclaration.Create(
                "sort",
                "void (std::__wrap_iter<std::pair<int, int> *>, std::__wrap_iter<std::pair<int, int> *>)",
                [
                    new ParameterVariableDeclaration
                    {
                        Id = "external_sort_param1",
                        Name = "begin",
                        Type = "std::__wrap_iter<std::pair<int, int> *>"
                    },
                    new ParameterVariableDeclaration
                    {
                        Id = "external_sort_param2",
                        Name = "end",
                        Type = "std::__wrap_iter<std::pair<int, int> *>"
                    }
                ]
            )
    };

    public static FunctionDeclaration GetFunctionDeclaration(string functionName, string type)
    {
        var key = (functionName, type);
        if (_externalFunctions.TryGetValue(key, out var functionDeclaration))
        {
            return functionDeclaration;
        }
        throw new KeyNotFoundException($"External function '{functionName}' with type '{type}' not found.");
    }
}