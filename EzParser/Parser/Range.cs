namespace EzParser.Parser
{
    internal class Range : BaseParser
    {
        private readonly char _from, _to;

        public Range(char @from, char to)
        {
            _from = @from;
            _to = to;
        }

        protected override AstNode ParseImpl(Slice input, Context ctx)
        {

            return AstNodeBuilder.Condition(ParserType.Range, input.Length > 0 && input[0] >= _from && input[0] <= _to, input, 1);
        }
    }
}
