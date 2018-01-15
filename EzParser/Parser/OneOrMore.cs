using System.Collections.Generic;
using System.Linq;

namespace EzParser.Parser
{
    internal class OneOrMore  :BaseParser
    {
        private readonly IParser _parser;

        public OneOrMore(IParser parser)
        {
            _parser = parser;
        }

        protected override AstNode ParseImpl(Slice input, Context ctx)
        {
            var children = new List<AstNode>();
            var remaining = input;
            AstNode childResult;

            while ((childResult = _parser.Parse(remaining, ctx)).Success)
            {
                children.Add(childResult);
                remaining = remaining.From(childResult.Match.Length);
            }

            return AstNodeBuilder.Condition(ParserType.OneOrMore, children.Any(), input, input.Length - remaining.Length, children);
        }
    }
}
