namespace EzParser.Parser
{
    internal class Decimate : BaseParser
    {
        private readonly IParser _parser;

        public Decimate(IParser parser)
        {
            _parser = parser;
        }

        protected override AstNode ParseImpl(Slice input, Context ctx)
        {
            var result = _parser.Parse(input, ctx);
            return AstNodeBuilder.Condition(result.Type, result.Success, input, result.Match);
        }
    }
}
