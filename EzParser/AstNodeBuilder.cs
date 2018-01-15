using System.Collections.Generic;
using Children = System.Collections.Generic.IReadOnlyList<EzParser.AstNode>;

namespace EzParser
{
    internal static class AstNodeBuilder
    {
        private static readonly Children NoChildren = new List<AstNode>();

        public static AstNode Failure(
            string type,
            Slice input)
        {
            return new AstNode(
                type,
                input[0, 0],
                false,
                NoChildren);
        }

        public static AstNode Success(
            string type,
            Slice input,
            int length,
            Children children = null)
        {
            return new AstNode(
                type,
                input[0, length],
                true,
                children ?? NoChildren);
        }

        public static AstNode Success(
            string type,
            Slice input,
            Slice match,
            Children children = null)
        {
            return new AstNode(
                type,
                match,
                true,
                children ?? NoChildren);
        }

        public static AstNode Condition(
            string type,
            bool condition,
            Slice input,
            int lengthIfValid,
            Children children = null)
        {
            return condition
                ? Success(type, input, lengthIfValid, children)
                : Failure(type, input);
        }

        public static AstNode Condition(
            string type,
            bool condition,
            Slice input,
            Slice match,
            Children children = null)
        {
            return condition
                ? Success(type, input, match, children)
                : Failure(type, input);
        }
    }
}
