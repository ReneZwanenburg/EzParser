namespace EzParser.Parser
{
    internal class Letter : BaseParser
    {
        protected override AstNode ParseImpl(Slice input, Context ctx)
        {
            return AstNodeBuilder.Condition(
                ParserType.Letter,
                input.Length > 0 && char.IsLetter(input[0]),
                input,
                1);
        }
    }
}
