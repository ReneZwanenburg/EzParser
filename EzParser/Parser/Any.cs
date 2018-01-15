namespace EzParser.Parser
{
    internal class Any : BaseParser
    {
        protected override AstNode ParseImpl(Slice input, Context ctx)
        {
            return AstNodeBuilder.Condition(ParserType.Any, input.Length > 0, input, 1);
        }
    }
}
