namespace DynamicWhereBuilder.Models.Sequence
{
    internal class LogicalOperatorOrSymbol : ILogicalOperatorSymbol, ISequenceSymbol
    {
        public int QueryPartIndex { get; set; }

        internal LogicalOperatorOrSymbol(int queryIndex)
        {
            this.QueryPartIndex = queryIndex;
        }
    }
}
