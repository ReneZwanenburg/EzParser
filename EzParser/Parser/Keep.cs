namespace EzParser.Parser
{
    internal class Keep : BaseParser
    {
        private readonly IParser _parser;

        public Keep(IParser parser)
        {
            _parser = parser;
        }

        protected override AstNode ParseImpl(Slice input, Context ctx)
        {
            var result = _parser.Parse(input, ctx);
            
            return new AstNode(
                result.Type.TrimStart('_'),
                result.Match,
                result.Success,
                result.Children);
        }
    }
}
