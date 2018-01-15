namespace EzParser.Parser
{
    internal class NonTerminal : BaseParser
    {
        private readonly string _type;
        private readonly IParser _parser;

        public NonTerminal(string type, IParser parser)
        {
            _type = type;
            _parser = parser;
        }

        protected override AstNode ParseImpl(Slice input, Context ctx)
        {
            var result = _parser.Parse(input, ctx);
            return AstNodeBuilder.Condition(_type, result.Success, input, result.Match, new []{ result });
        }
    }
}
