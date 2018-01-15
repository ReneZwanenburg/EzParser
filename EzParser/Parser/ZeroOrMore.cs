using System.Collections.Generic;

namespace EzParser.Parser
{
    internal class ZeroOrMore : BaseParser
    {
        private readonly IParser _parser;

        public ZeroOrMore(IParser parser)
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

            return AstNodeBuilder.Success(ParserType.ZeroOrMore, input, input.Length - remaining.Length, children);
        }
    }
}
