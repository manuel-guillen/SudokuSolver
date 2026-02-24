namespace PuzzleSolver.Core.Extensions;

using System.Collections.Generic;

public static class EnumerableExtensions
{
    public static string ToJoinedString<T>(this IEnumerable<T> source, string sep) => string.Join(sep, source);
}
