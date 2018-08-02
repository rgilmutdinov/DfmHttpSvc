using System;
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

            CommonTokenStream ts = (CommonTokenStream) this._calcParser.InputStream;

            Assert.AreEqual(type, ts.Get(0).Type);
            Assert.AreEqual(0, this._calcParser.NumberOfSyntaxErrors);
        }

        [Test]
        [TestCase("1", typeof(CalcParser.LiteralExpressionContext))]
        [TestCase("1.0", typeof(CalcParser.LiteralExpressionContext))]
        [TestCase("[2013-01-02]", typeof(CalcParser.LiteralExpressionContext))]
        [TestCase("1+1", typeof(CalcParser.AdditiveExpressionContext))]
        [TestCase("1-1", typeof(CalcParser.AdditiveExpressionContext))]
        [TestCase("1*1", typeof(CalcParser.MultiplicativeExpressionContext))]
        [TestCase("1/1", typeof(CalcParser.MultiplicativeExpressionContext))]
        [TestCase("2^2", typeof(CalcParser.PowerExpressionContext))]
        [TestCase("abs(1)", typeof(CalcParser.AbsExpressionContext))]
        [TestCase("sgn(1)", typeof(CalcParser.SgnExpressionContext))]
        [TestCase("sqrt(1)", typeof(CalcParser.SqrtExpressionContext))]
        [TestCase("e", typeof(CalcParser.ConstantExpressionContext))]
        [TestCase("pi", typeof(CalcParser.ConstantExpressionContext))]
        [TestCase("$FIELD(F1)", typeof(CalcParser.FieldExpressionContext))]
        [TestCase("$VAR(SYSTIME)", typeof(CalcParser.VarExpressionContext))]
        [TestCase("$NORMD(F1)", typeof(CalcParser.NormdExpressionContext))]
        [TestCase("$FLDLEN(F1)", typeof(CalcParser.FldlenExpressionContext))]
        [TestCase("1=1", typeof(CalcParser.EqualityExpressionContext))]
        [TestCase("1<>1", typeof(CalcParser.EqualityExpressionContext))]
        [TestCase("1>1", typeof(CalcParser.RelationalExpressionContext))]
        [TestCase("1<1", typeof(CalcParser.RelationalExpressionContext))]
        [TestCase("1>=1", typeof(CalcParser.RelationalExpressionContext))]
        [TestCase("1<=1", typeof(CalcParser.RelationalExpressionContext))]
        [TestCase("$FLDLEN(F1)", typeof(CalcParser.FldlenExpressionContext))]
        public void TestExpressionContextType(string value, Type expectedType)
        {
            Setup(value);

            CalcParser.ExpressionContext expression = this._calcParser.expression();
            Assert.AreEqual(expectedType, expression.GetType());
        }

        [Test]
        public void TestExpressionPowTypes()
        {
            Setup("5^3^2");

            this._calcParser.expression();

            CommonTokenStream ts = (CommonTokenStream) this._calcParser.InputStream;

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
        public void TestUnaryMinusType(string value, int type)
        {
            Setup(value);

            this._calcParser.expression();

            CommonTokenStream ts = (CommonTokenStream) this._calcParser.InputStream;

            Assert.AreEqual(CalcLexer.Minus, ts.Get(0).Type);
            Assert.AreEqual(type, ts.Get(1).Type);

            Assert.AreEqual(0, this._calcParser.NumberOfSyntaxErrors);
        }

        [Test]
        public void TestVarExpressionType()
        {
            Setup("$VAR(FIELD)");

            this._calcParser.expression();

            CommonTokenStream ts = (CommonTokenStream) this._calcParser.InputStream;

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
        public void TestIntegerAdditiveExpressions(string expression, int expectedResult)
        {
            Setup(expression);

            CalcParser.ExpressionContext context = this._calcParser.expression();

            CalcVisitor visitor = new CalcVisitor();
            Argument result = visitor.Visit(context);

            Assert.AreEqual(true, result.IsInteger);
            Assert.AreEqual(expectedResult, result.ToInteger());
        }

        [Test]
        [TestCase("2^2", 4)]
        [TestCase("2^2^2", 16)]
        [TestCase("(-2)^2", 4)]
        [TestCase("4^0.5", 2)]
        public void TestPowerExpressions(string expression, int expectedResult)
        {
            Setup(expression);

            CalcParser.ExpressionContext context = this._calcParser.expression();

            CalcVisitor visitor = new CalcVisitor();
            Argument result = visitor.Visit(context);

            Assert.AreEqual(true, result.IsInteger);
            Assert.AreEqual(expectedResult, result.ToInteger());
        }

        [Test]
        [TestCase("abs(2)", 2)]
        [TestCase("ABS(-2)", 2)]
        [TestCase("Abs(-1.1)", 1.1)]
        [TestCase("abs(1.1)", 1.1)]
        public void TestAbsExpressions(string expression, double expectedResult)
        {
            Setup(expression);

            CalcParser.ExpressionContext context = this._calcParser.expression();

            CalcVisitor visitor = new CalcVisitor();
            Argument result = visitor.Visit(context);

            Assert.AreEqual(expectedResult, result.ToDouble());
        }

        [Test]
        [TestCase("sgn(2)", 1)]
        [TestCase("SGN(-2)", -1)]
        [TestCase("Sgn(-1.1)", -1)]
        [TestCase("sgn(1.1)", 1)]
        [TestCase("sgn(0)", 0)]
        public void TestSngExpressions(string expression, double expectedResult)
        {
            Setup(expression);

            CalcParser.ExpressionContext context = this._calcParser.expression();

            CalcVisitor visitor = new CalcVisitor();
            Argument result = visitor.Visit(context);

            Assert.AreEqual(expectedResult, result.ToDouble());
        }

        [Test]
        [TestCase("sqrt(4)", 2)]
        [TestCase("SQRT(1)", 1)]
        [TestCase("Sqrt(0.81)", 0.9)]
        public void TestSrqtExpressions(string expression, double expectedResult)
        {
            Setup(expression);

            CalcParser.ExpressionContext context = this._calcParser.expression();

            CalcVisitor visitor = new CalcVisitor();
            Argument result = visitor.Visit(context);

            Assert.AreEqual(expectedResult, result.ToDouble());
        }

        [Test]
        [TestCase("1*2", 2)]
        [TestCase("1 * 2 * 3", 6)]
        [TestCase("1/2/3", 0)]
        [TestCase("1/2*3", 0)]
        [TestCase("1 / 2 * 3", 0)]
        [TestCase("(2 * 2) / (2 * 2)", 1)]
        [TestCase("(1 / 2) * (3 / 4)", 0)]
        [TestCase("(3 * 4) / (2 * 3)", 2)]
        public void TestIntegerMultiplicativeExpressions(string expression, int expectedResult)
        {
            Setup(expression);

            CalcParser.ExpressionContext context = this._calcParser.expression();

            CalcVisitor visitor = new CalcVisitor();
            Argument result = visitor.Visit(context);

            Assert.True(result.IsInteger);
            Assert.AreEqual(expectedResult, result.ToInteger());
        }

        [Test]
        [TestCase("-2", -2)]
        [TestCase("2 * -2", -4)]
        [TestCase("-2 + -2", -4)]
        [TestCase("-(-1)", 1)]
        public void TestUnaryMinusExpression(string expression, int expectedResult)
        {
            Setup(expression);

            CalcParser.ExpressionContext context = this._calcParser.expression();

            CalcVisitor visitor = new CalcVisitor();
            Argument result = visitor.Visit(context);

            Assert.True(result.IsInteger);
            Assert.AreEqual(expectedResult, result.ToInteger());
        }

        [Test]
        [TestCase("2 + 2 * 2", 6)]
        [TestCase("2+2*2", 6)]
        [TestCase("(-2-2)*2", -8)]
        [TestCase("(1+2) *(3 + 4) - (7 / 2 - 1/2) * (-5 - 1)", 39)]
        [TestCase("((((8 - 1) + 3) * 6) - ((-3 + 7) * 2))", 52)]
        [TestCase("(3 * 3 ^ 4) / 3 ^ 3", 9)]
        public void TestIntegerComplexExpressions(string expression, int expectedResult)
        {
            Setup(expression);

            CalcParser.ExpressionContext context = this._calcParser.expression();

            CalcVisitor visitor = new CalcVisitor();
            Argument result = visitor.Visit(context);

            Assert.True(result.IsInteger);
            Assert.AreEqual(expectedResult, result.ToInteger());
        }

        [Test]
        [TestCase("2 <> 3", 1)]
        [TestCase("2 = 3", 0)]
        [TestCase("2 = 2", 1)]
        [TestCase("2 <> 2", 0)]
        [TestCase("2 + 1 = 2 + 1", 1)]
        public void TestEqualityExpressions(string expression, double expectedResult)
        {
            Setup(expression);

            CalcParser.ExpressionContext context = this._calcParser.expression();

            CalcVisitor visitor = new CalcVisitor();
            Argument result = visitor.Visit(context);

            Assert.True(result.IsDouble);
            Assert.AreEqual(expectedResult, result.ToDouble());
        }

        [Test]
        [TestCase("2 < 3", 1)]
        [TestCase("2 > 3", 0)]
        [TestCase("2 <= 2", 1)]
        [TestCase("2 < 2", 0)]
        [TestCase("2 >= 2", 1)]
        [TestCase("2 > 2", 0)]
        [TestCase("2 + 1 >= 2 + 1", 1)]
        [TestCase("2 + 1 > 2 + 1", 0)]
        [TestCase("2 + 1 <= 2 + 1", 1)]
        [TestCase("2 + 1 < 2 + 1", 0)]
        public void TestRelationalExpressions(string expression, double expectedResult)
        {
            Setup(expression);

            CalcParser.ExpressionContext context = this._calcParser.expression();

            CalcVisitor visitor = new CalcVisitor();
            Argument result = visitor.Visit(context);

            Assert.True(result.IsDouble);
            Assert.AreEqual(expectedResult, result.ToDouble());
        }

        [Test]
        [TestCase("PI", Math.PI)]
        [TestCase("pi", Math.PI)]
        [TestCase("E", Math.E)]
        public void TestConstantExpressions(string expression, double expectedResult)
        {
            Setup(expression);

            CalcParser.ExpressionContext context = this._calcParser.expression();

            CalcVisitor visitor = new CalcVisitor();
            Argument result = visitor.Visit(context);

            Assert.True(result.IsDouble);
            Assert.AreEqual(expectedResult, result.ToDouble(), 1e-10);
        }

        [Test]
        [TestCase("0.1 * .5", 0.05)]
        [TestCase("2.0 / -0.5 + 21.7 / ((3.22 - 1.22) * 3.5)", -0.9)]
        public void TestDoubleComplexExpressions(string expression, double expectedResult)
        {
            Setup(expression);

            CalcParser.ExpressionContext context = this._calcParser.expression();

            CalcVisitor visitor = new CalcVisitor();
            Argument result = visitor.Visit(context);

            Assert.True(result.IsDouble);
            Assert.AreEqual(expectedResult, result.ToDouble(), 1e-10);
        }

        [Test]
        [TestCase("[2013-10-10] + 10", "2013-10-20")]
        [TestCase("[2013-10-10] + 1 - 1", "2013-10-10")]
        [TestCase("[2013-10-10] - [2013-10-09] + [2013-10-09]", "2013-10-10")]
        public void TestDateExpressionsWithDateResult(string expression, string expectedResult)
        {
            Setup(expression);

            CalcParser.ExpressionContext context = this._calcParser.expression();

            CalcVisitor visitor = new CalcVisitor();
            Argument result = visitor.Visit(context);

            Assert.True(result.IsDate);
            Assert.False(result.IsInteger);
            Assert.False(result.IsDouble);
            Assert.IsNotNull(result.ToDate());
            Assert.AreEqual(expectedResult, result.ToDate().Value.ToString("yyyy-MM-dd"));
        }

        [Test]
        [TestCase("[2013-10-10] - [2013-10-09]", 1)]
        [TestCase("([2013-10-12] - [2013-10-09])*([2013-10-11] - [2013-10-09])", 6)]
        public void TestDateExpressionsWithIntegerResult(string expression, int expectedResult)
        {
            Setup(expression);

            CalcParser.ExpressionContext context = this._calcParser.expression();

            CalcVisitor visitor = new CalcVisitor();
            Argument result = visitor.Visit(context);

            Assert.True(result.IsInteger);
            Assert.True(result.IsDouble);
            Assert.False(result.IsDate);
            Assert.AreEqual(expectedResult, result.ToInteger());
        }
    }
}
