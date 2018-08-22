namespace DynamicWhereBuilder.Models.Sequence
{
    internal class LogicalOperatorAndSymbol : ILogicalOperatorSymbol, ISequenceSymbol
    {
        public int QueryPartIndex { get; set; }

        internal LogicalOperatorAndSymbol(int queryIndex)
        {
            this.QueryPartIndex = queryIndex;
        }
    }
}
