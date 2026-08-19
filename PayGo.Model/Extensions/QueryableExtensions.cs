using PayGo.Model.Resources;

namespace PayGo.Model.Extensions;

/// <summary>
/// Extension methods for applying pagination to any IQueryable source.
/// Keeps pagination logic out of services and in a single, testable place.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Applies PageNumber and PageSize from PaginationParameters to an IQueryable.
    /// Usage: query.ApplyPagination(parameters)
    /// </summary>
    public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> source, PaginationParameters parameters)
    {
        if (parameters == null) throw new ArgumentNullException(nameof(parameters));

        return source
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize);
    }

    /// <summary>
    /// Applies manual page/size pagination to any IQueryable.
    /// </summary>
    public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> source, int pageNumber, int pageSize)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 20;

        return source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);
    }
}
