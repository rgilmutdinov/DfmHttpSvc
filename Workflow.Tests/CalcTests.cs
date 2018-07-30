using Antlr4.Runtime;
using NUnit.Framework;
using Workflow.Expressions;

namespace Workflow.Tests
{
    public class CalcTests
    {
        private CalcLexer _calcLexer;
        private CalcParser _calcParser;

        private void Setup(string input)
        {
            AntlrInputStream inputStream = new AntlrInputStream(input);
            this._calcLexer = new CalcLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(this._calcLexer);
            this._calcParser = new CalcParser(commonTokenStream)
            {
                BuildParseTree = true
            };
        }

        [Test]
        [TestCase("1",           CalcLexer.IntegerLiteral)]
        [TestCase("10",          CalcLexer.IntegerLiteral)]
        [TestCase("0",           CalcLexer.IntegerLiteral)]
        [TestCase("1.5",         CalcLexer.DecimalLiteral)]
        [TestCase("10.99",       CalcLexer.DecimalLiteral)]
        [TestCase("0.3",         CalcLexer.DecimalLiteral)]
        [TestCase("'Some text'", CalcLexer.StringLiteral)]
        [TestCase("''",          CalcLexer.StringLiteral)]
        [TestCase("'\''",        CalcLexer.StringLiteral)]
        [TestCase("FIELD",       CalcLexer.Identifier)]
        [TestCase("_123",        CalcLexer.Identifier)]
        [TestCase("abc_ABC_123", CalcLexer.Identifier)]
        [TestCase("pi",          CalcLexer.ConstPi)]
        [TestCase("PI",          CalcLexer.ConstPi)]
        [TestCase("e",           CalcLexer.ConstE)]
        [TestCase("E",           CalcLexer.ConstE)]
        public void TestLexerRules(string value, int type)
        {
            Setup(value);

            this._calcParser.expression();

            CommonTokenStream ts = (CommonTokenStream) _calcParser.InputStream;

            Assert.AreEqual(type, ts.Get(0).Type);
            Assert.AreEqual(0, this._calcParser.NumberOfSyntaxErrors);
        }

        [Test]
        public void TestExpressionPow()
        {
            Setup("5^3^2");

            this._calcParser.expression();

            CommonTokenStream ts = (CommonTokenStream) _calcParser.InputStream;

            Assert.AreEqual(CalcLexer.IntegerLiteral, ts.Get(0).Type);
            Assert.AreEqual(CalcLexer.Power, ts.Get(1).Type);
            Assert.AreEqual(CalcLexer.IntegerLiteral, ts.Get(2).Type);
            Assert.AreEqual(CalcLexer.Power, ts.Get(3).Type);
            Assert.AreEqual(CalcLexer.IntegerLiteral, ts.Get(4).Type);

            Assert.AreEqual(0, this._calcParser.NumberOfSyntaxErrors);
        }

        [Test]
        [TestCase("-1",    CalcLexer.IntegerLiteral)]
        [TestCase("-9.99", CalcLexer.DecimalLiteral)]
        public void TestUnaryMinus(string value, int type)
        {
            Setup(value);

            this._calcParser.expression();

            CommonTokenStream ts = (CommonTokenStream) _calcParser.InputStream;

            Assert.AreEqual(CalcLexer.Minus, ts.Get(0).Type);
            Assert.AreEqual(type, ts.Get(1).Type);

            Assert.AreEqual(0, this._calcParser.NumberOfSyntaxErrors);
        }

        [Test]
        public void TestVarExpression()
        {
            Setup("$VAR(FIELD)");

            this._calcParser.expression();

            CommonTokenStream ts = (CommonTokenStream)_calcParser.InputStream;

            Assert.AreEqual(CalcLexer.Var, ts.Get(0).Type);
            Assert.AreEqual(CalcLexer.OpenParen, ts.Get(1).Type);
            Assert.AreEqual(CalcLexer.Identifier, ts.Get(2).Type);
            Assert.AreEqual(CalcLexer.CloseParen, ts.Get(3).Type);

            Assert.AreEqual(0, this._calcParser.NumberOfSyntaxErrors);
        }

        [Test]
        [TestCase("1+2", 3)]
        [TestCase("1 + 2 + 3", 6)]
        [TestCase("1+2+3", 6)]
        [TestCase("1-2+3", 2)]
        [TestCase("1 - 2 + 3", 2)]
        [TestCase("(2 + 2) - (2 + 2)", 0)]
        [TestCase("(1 - 2) + (3 - 4)", -2)]
        public void TestIntegerAdditiveExpressions(string expression, int exprectedResult)
        {
            Setup(expression);

            CalcParser.ExpressionContext context = this._calcParser.expression();

            CalcVisitor visitor = new CalcVisitor();
            int actualResult = visitor.Visit(context);

            Assert.AreEqual(exprectedResult, actualResult);
        }

    }
}
