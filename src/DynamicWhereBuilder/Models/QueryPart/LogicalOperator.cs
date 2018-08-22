namespace DynamicWhereBuilder.Models.QueryPart
{
    /// <summary>
    /// Logical operator
    /// </summary>
    public enum LogicalOperator
    {
        /// <summary>
        /// Will be converted to &&
        /// </summary>
        And,

        /// <summary>
        /// Will be converted to ||
        /// </summary>
        Or
    }
}
