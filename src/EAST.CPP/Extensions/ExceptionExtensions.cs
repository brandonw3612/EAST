namespace EAST.CPP.Extensions;

public static class ExceptionExtensions
{
    public static T? VisitException<T>(this Exception e) where T : Exception
    {
        return e as T ?? e.InnerException?.VisitException<T>();
    }
}