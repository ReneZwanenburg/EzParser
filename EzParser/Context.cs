using System;
using System.Collections.Generic;

namespace EzParser
{
    public class Context
    {
        // Packrat parsing cache.
        private readonly Dictionary<Tuple<IParser, int>, AstNode> _cache
            = new Dictionary<Tuple<IParser, int>, AstNode>();

        private readonly bool _useMemoization;

        internal Context(
            bool useMemoization = true)
        {
            _useMemoization = useMemoization;
        }

        internal bool GetCached(
            IParser parser,
            Slice input,
            out AstNode result)
        {
            return _cache.TryGetValue(
                Tuple.Create(parser, input.StartIndex),
                out result);
        }

        internal void Add(
            IParser parser,
            Slice input,
            AstNode result)
        {
            if (!_useMemoization)
                return;

            _cache.Add(
                Tuple.Create(parser, input.StartIndex),
                result);
        }
    }
}
