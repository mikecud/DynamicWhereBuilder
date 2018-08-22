using System;
using System.Text;

namespace DynamicWhereBuilder.Exceptions
{
    public class ParseException : Exception
    {
        private int? QueryPartIndex { get; set; }

        internal ParseException(string message, int queryPartIndex) : base(message)
        {
            this.QueryPartIndex = queryPartIndex;
        }

        internal ParseException(string message) : base(message)
        {
        }

        public override string Message
        {
            get
            {
                var builder = new StringBuilder();
                builder.Append(base.Message);

                if (this.QueryPartIndex.HasValue)
                {
                    builder.AppendLine($"QueryPart index: {this.QueryPartIndex}");
                }

                return builder.ToString();
            }
        }
    }
}
