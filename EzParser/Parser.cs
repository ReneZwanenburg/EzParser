using System;
using System.Collections.Generic;
using System.Linq;
using static EzParser.AstNodeBuilder;

namespace EzParser
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

    internal class Any : BaseParser
    {
        protected override AstNode ParseImpl(Slice input, Context ctx)
        {
            return Condition(ParserType.Any, input.Length > 0, input, 1);
        }
    }

    internal class CaseInsensitiveTerminal : BaseParser
    {
        private readonly string _literal;

        internal CaseInsensitiveTerminal(string literal)
        {
            _literal = literal.ToUpperInvariant();
        }

        protected override AstNode ParseImpl(Slice input, Context ctx)
        {
            if (input.Length < _literal.Length)
                return Failure(ParserType.Terminal, input);

            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var i = 0; i < _literal.Length; ++i)
            {
                if (char.ToUpperInvariant(input[i]) != _literal[i])
                    return Failure(ParserType.Terminal, input);
            }

            return Success(ParserType.Terminal, input, _literal.Length);
        }
    }

    internal class Choice : BaseParser
    {
        private readonly IParser[] _options;

        public Choice(IParser[] options)
        {
            _options = options;
        }

        protected override AstNode ParseImpl(Slice input, Context ctx)
        {
            foreach (var option in _options)
            {
                var result = option.Parse(input, ctx);

                if (result.Success)
                    return Success(ParserType.Choice, input, result.Match, new[] { result });
            }

            return Failure(ParserType.Choice, input);
        }
    }

    internal class Decimate : BaseParser
    {
        private readonly IParser _parser;

        public Decimate(IParser parser)
        {
            _parser = parser;
        }

        protected override AstNode ParseImpl(Slice input, Context ctx)
        {
            var result = _parser.Parse(input, ctx);
            return Condition(result.Type, result.Success, input, result.Match);
        }
    }

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

    internal class Keep : BaseParser
    {
        private readonly IParser _parser;

        public Keep(IParser parser)
        {
            _parser = parser;
        }

        protected override AstNode ParseImpl(Slice input, Context ctx)
        {
            var result = _parser.Parse(input, ctx);

            return new AstNode(
                result.Type.TrimStart('_'),
                result.Match,
                result.Success,
                result.Children);
        }
    }

    internal class Letter : BaseParser
    {
        protected override AstNode ParseImpl(Slice input, Context ctx)
        {
            return Condition(
                ParserType.Letter,
                input.Length > 0 && char.IsLetter(input[0]),
                input,
                1);
        }
    }

    internal class Lookahead : BaseParser
    {
        private readonly IParser _parse;

        public Lookahead(IParser parse)
        {
            _parse = parse;
        }

        protected override AstNode ParseImpl(Slice input, Context ctx)
        {
            return Condition(ParserType.Lookahead, _parse.Parse(input, ctx).Success, input, 0);
        }
    }

    internal class NonTerminal : BaseParser
    {
        private readonly string _type;
        private readonly IParser _parser;

        public NonTerminal(string type, IParser parser)
        {
            _type = type;
            _parser = parser;
        }

        protected override AstNode ParseImpl(Slice input, Context ctx)
        {
            var result = _parser.Parse(input, ctx);
            return Condition(_type, result.Success, input, result.Match, new[] { result });
        }
    }

    internal class Not : BaseParser
    {
        private readonly IParser _parser;
        internal Not(IParser parser)
        {
            _parser = parser;
        }

        protected override AstNode ParseImpl(Slice input, Context ctx)
        {
            return Condition(ParserType.Not, !_parser.Parse(input, ctx).Success, input, 0);
        }
    }

    internal class OneOrMore : BaseParser
    {
        private readonly IParser _parser;

        public OneOrMore(IParser parser)
        {
            _parser = parser;
        }

        protected override AstNode ParseImpl(Slice input, Context ctx)
        {
            var children = new List<AstNode>();
            var remaining = input;
            AstNode childResult;

            while ((childResult = _parser.Parse(remaining, ctx)).Success)
            {
                children.Add(childResult);
                remaining = remaining.From(childResult.Match.Length);
            }

            return Condition(ParserType.OneOrMore, children.Any(), input, input.Length - remaining.Length, children);
        }
    }

    internal class Optional : BaseParser
    {
        private readonly IParser _parser;

        public Optional(IParser parser)
        {
            _parser = parser;
        }

        protected override AstNode ParseImpl(Slice input, Context ctx)
        {
            var result = _parser.Parse(input, ctx);

            return result.Success
                ? Success(ParserType.Optional, input, result.Match, new[] { result })
                : Success(ParserType.Optional, input, 0);
        }
    }

    internal class Range : BaseParser
    {
        private readonly char _from, _to;

        public Range(char @from, char to)
        {
            _from = @from;
            _to = to;
        }

        protected override AstNode ParseImpl(Slice input, Context ctx)
        {

            return Condition(ParserType.Range, input.Length > 0 && input[0] >= _from && input[0] <= _to, input, 1);
        }
    }

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
                    return Failure(ParserType.Sequence, input);

                children.Add(subResult);
                remaining = remaining.From(subResult.Match.Length);
            }

            return Success(ParserType.Sequence, input, input.Length - remaining.Length, children);
        }
    }

    internal class Terminal : BaseParser
    {
        private readonly string _literal;

        internal Terminal(string literal)
        {
            _literal = literal;
        }

        protected override AstNode ParseImpl(Slice input, Context ctx)
        {
            if (input.Length < _literal.Length)
                return Failure(ParserType.Terminal, input);

            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var i = 0; i < _literal.Length; ++i)
            {
                if (input[i] != _literal[i])
                    return Failure(ParserType.Terminal, input);
            }

            return Success(ParserType.Terminal, input, _literal.Length);
        }
    }

    // Common enough that it was worth adding as a special case instead of going
    // through a class parser.
    internal class Whitespace : BaseParser
    {
        protected override AstNode ParseImpl(Slice input, Context ctx)
        {
            return Condition(
                ParserType.Whitespace,
                input.Length > 0 && char.IsWhiteSpace(input[0]),
                input,
                1);
        }
    }

    internal class ZeroOrMore : BaseParser
    {
        private readonly IParser _parser;

        public ZeroOrMore(IParser parser)
        {
            _parser = parser;
        }

        protected override AstNode ParseImpl(Slice input, Context ctx)
        {
            var children = new List<AstNode>();
            var remaining = input;
            AstNode childResult;

            while ((childResult = _parser.Parse(remaining, ctx)).Success)
            {
                children.Add(childResult);
                remaining = remaining.From(childResult.Match.Length);
            }

            return Success(ParserType.ZeroOrMore, input, input.Length - remaining.Length, children);
        }
    }
}
