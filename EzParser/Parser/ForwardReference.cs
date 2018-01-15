using System;

namespace EzParser.Parser
{
    internal class ForwardReference : IParser
    {
        private readonly Func<IParser> _reference;
        private IParser _parser;

        public ForwardReference(Func<IParser> reference)
        {
            _reference = reference;
        }

        public AstNode Parse(Slice input, Context ctx)
        {
            return (_parser ?? (_parser = _reference())).Parse(input, ctx);
        }
    }
}
