namespace EzParser.Parser
{
    internal class Choice : BaseParser
    {
        private readonly IParser[] _options;

        public Choice(IParser[] options)
        {
            _options = options;
        }

        protected override AstNode ParseImpl(Slice input, Context ctx)
        {
            foreach (var option in _options)
            {
                var result = option.Parse(input, ctx);

                if (result.Success)
                    return AstNodeBuilder.Success(ParserType.Choice, input, result.Match, new[] { result });
            }

            return AstNodeBuilder.Failure(ParserType.Choice, input);
        }
    }
}
