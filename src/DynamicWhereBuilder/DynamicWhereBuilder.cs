using DynamicWhereBuilder.Models.QueryPart;
using DynamicWhereBuilder.Services.Implementations;
using DynamicWhereBuilder.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicWhereBuilder.Tests")]

namespace DynamicWhereBuilder
{
    public static class WhereExtensions
    {
        private static IWhereExecutor<T> GetWhereExecutor<T>() => new WhereExecutor<T>(
                new QueryPartsValidator<T>(),
                new SequenceOfSymbolsBuilder<T>(),
                new SequenceOfSymbolsValidator<T>(),
                new ExpressionCollectionBuilder<T>(),
                new ExpressionBuilder<T>());

        /// <summary>
        /// Filters a sequence of values based on a predicate
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="source">An System.Linq.IQueryable`1 to filter.</param>
        /// <param name="queryParts">Collection of QueryParts to build a conditon for testing of each element.</param>
        /// <returns>An System.Linq.IQueryable`1 that contains elements from the input sequence that satisfy the condition specified in QueryParts.</returns>
        public static IQueryable<T> Where<T>(this IQueryable<T> source, params QueryPart<T>[] queryParts)
        {
            return Where(source, queryParts?.AsEnumerable());
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="source">An System.Linq.IQueryable`1 to filter.</param>
        /// <param name="queryParts">Collection of QueryParts to build a conditon for testing of each element.</param>
        /// <returns>An System.Linq.IQueryable`1 that contains elements from the input sequence that satisfy the condition specified in QueryParts.</returns>
        public static IQueryable<T> Where<T>(this IQueryable<T> query, IEnumerable<QueryPart<T>> queryParts)
        {
            return GetWhereExecutor<T>().Execute(query, queryParts);
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="source">An System.Collections.Generic.IEnumerable`1 to filter.</param>
        /// <param name="queryParts">Collection of QueryParts to build a conditon for testing of each element.</param>
        /// <returns>An System.Collections.Generic.IEnumerable`1 that contains elements from the input sequence that satisfy the condition specified in QueryParts.</returns>
        public static IEnumerable<T> Where<T>(this IEnumerable<T> source, params QueryPart<T>[] queryParts)
        {
            return Where(source, queryParts?.AsEnumerable());
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="source">An System.Collections.Generic.IEnumerable`1 to filter.</param>
        /// <param name="queryParts">Collection of QueryParts to build a conditon for testing of each element.</param>
        /// <returns>An System.Collections.Generic.IEnumerable`1 that contains elements from the input sequence that satisfy the condition specified in QueryParts.</returns>
        public static IEnumerable<T> Where<T>(this IEnumerable<T> collection, IEnumerable<QueryPart<T>> queryParts)
        {
            return GetWhereExecutor<T>().Execute(collection, queryParts);
        }
    }
}