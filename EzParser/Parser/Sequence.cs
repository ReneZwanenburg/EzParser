using System.Collections.Generic;

namespace EzParser.Parser
{
    internal class Sequence : BaseParser
    {
        private readonly IParser[] _elements;

        public Sequence(IParser[] elements)
        {
            _elements = elements;
        }

        protected override AstNode ParseImpl(Slice input, Context ctx)
        {
            var children = new List<AstNode>(_elements.Length);
            var remaining = input;

            foreach (var element in _elements)
            {
                var subResult = element.Parse(remaining, ctx);

                if (!subResult.Success)
                    return AstNodeBuilder.Failure(ParserType.Sequence, input);

                children.Add(subResult);
                remaining = remaining.From(subResult.Match.Length);
            }

            return AstNodeBuilder.Success(ParserType.Sequence, input, input.Length - remaining.Length, children);
        }
    }
}
