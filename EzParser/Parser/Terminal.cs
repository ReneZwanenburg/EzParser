namespace EzParser.Parser
{
    internal class Terminal : BaseParser
    {
        private readonly string _literal;

        internal Terminal(string literal)
        {
            _literal = literal;
        }

        protected override AstNode ParseImpl(Slice input, Context ctx)
        {
            if (input.Length < _literal.Length)
                return AstNodeBuilder.Failure(ParserType.Terminal, input);

            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var i = 0; i < _literal.Length; ++i)
            {
                if (input[i] != _literal[i])
                    return AstNodeBuilder.Failure(ParserType.Terminal, input);
            }

            return AstNodeBuilder.Success(ParserType.Terminal, input, _literal.Length);
        }
    }
}
