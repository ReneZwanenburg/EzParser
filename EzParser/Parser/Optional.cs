namespace EzParser.Parser
{
    internal class Optional : BaseParser
    {
        private readonly IParser _parser;

        public Optional(IParser parser)
        {
            _parser = parser;
        }

        protected override AstNode ParseImpl(Slice input, Context ctx)
        {
            var result = _parser.Parse(input, ctx);

            return result.Success
                ? AstNodeBuilder.Success(ParserType.Optional, input, result.Match, new[] { result })
                : AstNodeBuilder.Success(ParserType.Optional, input, 0);
        }
    }
}
