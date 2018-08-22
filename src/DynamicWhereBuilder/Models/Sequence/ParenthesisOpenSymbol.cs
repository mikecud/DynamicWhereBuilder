namespace DynamicWhereBuilder.Models.Sequence
{
    internal class ParenthesisOpenSymbol : IParenthesisSymbol, ISequenceSymbol
    {
        public int QueryPartIndex { get; set; }

        internal ParenthesisOpenSymbol(int queryIndex)
        {
            this.QueryPartIndex = queryIndex;
        }
    }
}
