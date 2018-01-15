namespace EzParser.Parser
{
    internal class Not : BaseParser
    {
        private readonly IParser _parser;
        internal Not(IParser parser)
        {
            _parser = parser;
        }

        protected override AstNode ParseImpl(Slice input, Context ctx)
        {
            return AstNodeBuilder.Condition(ParserType.Not, !_parser.Parse(input, ctx).Success, input, 0);
        }
    }
}
