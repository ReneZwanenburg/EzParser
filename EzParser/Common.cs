using static EzParser.ParserBuilder;

namespace EzParser
{
    public static class Common
    {
        public static IParser Integer(
            bool allowLeadingZero = false,
            bool allowNegative = true,
            string name = "Integer")
        {
            var parser = allowLeadingZero
                ? OneOrMore(Class("0-9"))
                : Choice(
                    T("0"),
                    Sequence(
                        Class("1-9"),
                        ZeroOrMore(Class("0-9"))
                    )
                );

            if (allowNegative)
                parser = Sequence(Optional(T("-")), parser);

            return AddName(parser, name);
        }

        public static IParser Decimal(
            bool allowLeadingZero = false,
            bool allowNegative = true,
            bool allowIntegerOnly = true,
            bool allowEmptyDecimalDigits = false, // Ex: 12. instead of 12.0
            string name = "Decimal")
        {
            var intPart = Integer(
                allowLeadingZero,
                allowNegative,
                null);

            var decimalDigitsPart = allowEmptyDecimalDigits
                ? ZeroOrMore(Class("0-9"))
                : OneOrMore(Class("0-9"));

            var decimalPart = Sequence(T("."), decimalDigitsPart);

            if (allowIntegerOnly)
                decimalPart = Optional(decimalPart);

            var parser = Sequence(intPart, decimalPart);

            return AddName(parser, name);
        }

        public static IParser Identifier(
            string name = "Identifier")
        {
            var parser = Sequence(
                Choice(T('_'), ParserBuilder.Letter),
                ZeroOrMore(Choice(T('_'), LetterOrDigit))
            );

            return AddName(parser, name);
        }

        public static IParser Delimited(
            IParser element,
            IParser delimiter)
        {
            return Sequence(
                element,
                ZeroOrMore(delimiter, element)
            );
        }

        private static IParser AddName(
            IParser parser,
            string name)
        {
            if (string.IsNullOrEmpty(name))
                return parser;

            return NonTerminal(name, parser);
        }
    }
}
