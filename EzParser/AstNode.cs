using System.Collections.Generic;

namespace EzParser
{
    public struct AstNode
    {
        public string Type { get; }
        public Slice Match { get; }
        public IReadOnlyList<AstNode> Children { get; }
        public bool Success { get; }

        public AstNode(
            string type,
            Slice match,
            bool success,
            IReadOnlyList<AstNode> children)
        {
            Type = type;
            Match = match;
            Success = success;
            Children = children;
        }
    }
}
