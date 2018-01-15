namespace EzParser.Parser
{
    // Common enough that it was worth adding as a special case instead of going
    // through a class parser.
    internal class Whitespace : BaseParser
    {
        protected override AstNode ParseImpl(Slice input, Context ctx)
        {
            return AstNodeBuilder.Condition(
                ParserType.Whitespace,
                input.Length > 0 && char.IsWhiteSpace(input[0]),
                input,
                1);
        }
    }
}
