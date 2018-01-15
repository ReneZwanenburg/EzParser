namespace EzParser.Parser
{
    internal class Lookahead : BaseParser
    {
        private readonly IParser _parse;

        public Lookahead(IParser parse)
        {
            _parse = parse;
        }

        protected override AstNode ParseImpl(Slice input, Context ctx)
        {
            return AstNodeBuilder.Condition(ParserType.Lookahead, _parse.Parse(input, ctx).Success, input, 0);
        }
    }
}
