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
                ? ParserBuilder.OneOrMore(ParserBuilder.Class("0-9"))
                : ParserBuilder.Choice(
                    ParserBuilder.T("0"),
                    ParserBuilder.Sequence(
                        ParserBuilder.Class("1-9"),
                        ParserBuilder.ZeroOrMore(ParserBuilder.Class("0-9"))
                    )
                );

            if (allowNegative)
            {
                parser = ParserBuilder.Sequence(
                    ParserBuilder.Optional(ParserBuilder.T("-")),
                    parser
                );
            }

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
                ? ParserBuilder.ZeroOrMore(ParserBuilder.Class("0-9"))
                : ParserBuilder.OneOrMore(ParserBuilder.Class("0-9"));

            var decimalPart = ParserBuilder.Sequence(
                ParserBuilder.T("."),
                decimalDigitsPart);

            if (allowIntegerOnly)
                decimalPart = ParserBuilder.Optional(decimalPart);

            var parser = ParserBuilder.Sequence(
                intPart,
                decimalPart);

            return AddName(parser, name);
        }

        public static IParser Identifier(
            string name = "Identifier")
        {
            var parser = ParserBuilder.Sequence(
                ParserBuilder.Choice(
                    ParserBuilder.T('_'),
                    ParserBuilder.Letter
                ),
                ParserBuilder.ZeroOrMore(ParserBuilder.Choice(
                    ParserBuilder.T('_'),
                    ParserBuilder.LetterOrDigit
                ))
            );

            return AddName(parser, name);
        }

        public static IParser Delimited(
            IParser element,
            IParser delimiter)
        {
            return ParserBuilder.Sequence(
                element,
                ParserBuilder.ZeroOrMore(ParserBuilder.Sequence(
                    delimiter,
                    element
                ))
            );
        }

        private static IParser AddName(
            IParser parser,
            string name)
        {
            if (string.IsNullOrEmpty(name))
                return parser;

            return ParserBuilder.NonTerminal(
                name,
                parser);
        }
    }
}
