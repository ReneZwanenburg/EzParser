using System.Collections.Generic;
using System.Linq;

namespace EzParser
{
    public static class InvokeParser
    {
        public static AstNode Parse(
            this IParser parser,
            Slice input)
        {
            var result = parser.Parse(input, new Context());
            
            result = Reduce(result);

            return result;
        }

        private static AstNode Reduce(AstNode node)
        {
            var reducedChildren = Reduce(node.Children);

            return new AstNode(
                node.Type,
                node.Match,
                node.Success,
                reducedChildren);
        }

        private static IReadOnlyList<AstNode> Reduce(IEnumerable<AstNode> children)
        {
            bool ShoudSkip(AstNode node) => node.Type.StartsWith("_");

            return children
                .SelectMany(a => ShoudSkip(a) ? Reduce(a.Children) : new[] {Reduce(a)})
                .ToList();
        }
    }
}
