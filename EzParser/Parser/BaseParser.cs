namespace EzParser.Parser
{
    internal abstract class BaseParser : IParser
    {
        public AstNode Parse(Slice input, Context ctx)
        {
            if (ctx.GetCached(this, input, out AstNode cached))
                return cached;

            var result = ParseImpl(input, ctx);

            ctx.Add(this, input, result);

            return result;
        }

        protected abstract AstNode ParseImpl(Slice input, Context ctx);
    }
}
