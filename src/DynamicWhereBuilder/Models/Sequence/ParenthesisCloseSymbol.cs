namespace DynamicWhereBuilder.Models.Sequence
{
    internal class ParenthesisCloseSymbol : IParenthesisSymbol, ISequenceSymbol
    {
        public int QueryPartIndex { get; set; }

        internal ParenthesisCloseSymbol(int queryIndex)
        {
            this.QueryPartIndex = queryIndex;
        }
    }
}
