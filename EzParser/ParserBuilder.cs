using System;
using System.Linq;
using EzParser.Parser;

namespace EzParser
{
    public static class ParserBuilder
    {
        public static IParser T(string terminal, bool caseSensitive = true)
        {
            return caseSensitive
                ? (IParser)new Terminal(terminal)
                : new CaseInsensitiveTerminal(terminal);
        }

        public static IParser T(char terminal, bool caseSensitive = true)
        {
            return T(terminal.ToString(), caseSensitive);
        }

        public static IParser Any { get; } = new Any();

        public static IParser Choice(params IParser[] options)
        {
            return options.Length == 1
                ? options.First()
                : new Choice(options);
        }

        public static IParser Lookahead(IParser parser)
        {
            return new Lookahead(parser);
        }

        public static IParser Lookahead(params IParser[] sequence)
        {
            return Lookahead(Sequence(sequence));
        }

        public static IParser Not(IParser parser)
        {
            return new Not(parser);
        }

        public static IParser Not(params IParser[] sequence)
        {
            return Not(Sequence(sequence));
        }

        public static IParser OneOrMore(IParser parser)
        {
            return new OneOrMore(parser);
        }

        public static IParser OneOrMore(params IParser[] sequence)
        {
            return OneOrMore(Sequence(sequence));
        }

        public static IParser ZeroOrMore(IParser parser)
        {
            return new ZeroOrMore(parser);
        }

        public static IParser ZeroOrMore(params IParser[] sequence)
        {
            return ZeroOrMore(Sequence(sequence));
        }

        public static IParser Optional(IParser parser)
        {
            return new Optional(parser);
        }

        public static IParser Optional(params IParser[] sequence)
        {
            return Optional(Sequence(sequence));
        }

        public static IParser Range(char from, char to)
        {
            return new Range(from, to);
        }

        public static IParser Letter { get; } = new Letter();

        public static IParser LetterOrDigit { get; } = Choice(
            Range('0', '9'),
            Letter);

        public static IParser Whitespace { get; } = new Whitespace();
        
        public static readonly IParser OptionalWhitespace =
            ZeroOrMore(Whitespace);

        public static IParser Sequence(params IParser[] elements)
        {
            return elements.Length == 1
                ? elements.First()
                : new Sequence(elements);
        }

        public static IParser Keep(IParser parser)
        {
            return new Keep(parser);
        }

        public static IParser NonTerminal(string type, IParser parser)
        {
            return new NonTerminal(type, parser);
        }

        public static IParser NonTerminal(string type, params IParser[] sequence)
        {
            return NonTerminal(type, Sequence(sequence));
        }

        public static IParser Decimate(IParser parser)
        {
            return new Decimate(parser);
        }

        public static IParser ForwardReference(Func<IParser> reference)
        {
            return new ForwardReference(reference);
        }

        private static readonly IParser ClassDefinitionParser = 
            OneOrMore(Choice(
                NonTerminal("RangePart", Sequence(Keep(Any), T('-'), Keep(Any))),
                Keep(Any)
            ));

        public static IParser Class(string classDefinition)
        {
            var definition = ClassDefinitionParser.Parse(classDefinition);

            if (!definition.Success) throw new ArgumentException($"Invalid class definition: {classDefinition}");

            var classParts = definition.Children;

            return Choice(classParts.Select(ParserForClassPart).ToArray());
        }

        private static IParser ParserForClassPart(AstNode node)
        {
            return node.Type == "RangePart"
                ? Range(node.Children[0].Match[0], node.Children[1].Match[0])
                : T(node.Match.Value);
        }

        public static IParser ParseFull(string name, IParser entryPoint)
        {
            return NonTerminal(name, Sequence(
                entryPoint,
                Not(Any)
            ));
        }
    }
}
