using DynamicWhereBuilder.Models.QueryPart;
using System;
using System.Linq.Expressions;

namespace DynamicWhereBuilder.Models.QueryPart
{
    /// <summary>
    /// Represents sentence that will be converted to Expression
    /// </summary>
    /// <typeparam name="T">Tye type of the elements of source</typeparam>
    public class QueryPart<T>
    {
        /// <summary>
        /// Initializes new instance
        /// </summary>
        /// <param name="initialParenthesis">Provide Parenthesis.Open for "(" or Parenthesis.Close for ")" or null if you want to skip it</param>
        /// <param name="expression">A function to test each element for a condition or null to skip it</param>
        /// <param name="endingParenthesis">Provide Parenthesis.Open for "(" or Parenthesis.Close for ")" or null if you want to skip it</param>
        /// <param name="logicalOperator">Provide LogicalOperator.And for "&&" or LogicalOperator.Or for "||" or null to skip it</param>
        public QueryPart(Parenthesis? initialParenthesis, Expression<Func<T, bool>> expression, Parenthesis? endingParenthesis, LogicalOperator? logicalOperator)
        {
            InitialParenthesis = initialParenthesis;
            Expression = expression;
            EndingParenthesis = endingParenthesis;
            LogicalOperator = logicalOperator;
        }

        /// <summary>
        /// First argument, parenthesis
        /// </summary>
        public Parenthesis? InitialParenthesis { get; private set; }

        /// <summary>
        /// Second argument, a function to test each element for a condition
        /// </summary>
        public Expression<Func<T, bool>> Expression { get; private set; }

        /// <summary>
        /// Third argument, parenthesis
        /// </summary>
        public Parenthesis? EndingParenthesis { get; private set; }

        /// <summary>
        /// Fourth argument, logical operator
        /// </summary>
        public LogicalOperator? LogicalOperator { get; private set; }
    }
}
