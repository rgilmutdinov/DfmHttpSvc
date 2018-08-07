using Antlr4.Runtime;
using NUnit.Framework;
using Workflow.Expressions;

namespace Workflow.Tests
{
    public class QueryVisitorTests
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
        [TestCase("2 + 2", "(2+2)")]
        [TestCase("(2 - 2)", "((2-2))")]
        [TestCase("2 - 2 + 2", "((2-2)+2)")]
        public void TestAdditiveExpressions(string expr, string expectedQuery)
        {
            Setup(expr);

            CalcParser.ExpressionContext context = this._calcParser.expression();

            QueryVisitor visitor = new QueryVisitor();
            string query = visitor.Visit(context);

            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        [TestCase("2 * 2", "(2*2)")]
        [TestCase("(2 * 2)", "((2*2))")]
        [TestCase("2 * 2 / 2", "((2*2)/nullif(2, 0))")]
        [TestCase("2 / 2 * 2", "((2/nullif(2, 0))*2)")]
        public void TestMultiplicativeExpressions(string expr, string expectedQuery)
        {
            Setup(expr);

            CalcParser.ExpressionContext context = this._calcParser.expression();

            QueryVisitor visitor = new QueryVisitor();
            string query = visitor.Visit(context);

            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        [TestCase("2 ^ 2", "(2^2)")]
        [TestCase("(2 ^ 3)", "((2^3))")]
        public void TestPowerExpressions(string expr, string expectedQuery)
        {
            Setup(expr);

            CalcParser.ExpressionContext context = this._calcParser.expression();

            QueryVisitor visitor = new QueryVisitor();
            string query = visitor.Visit(context);

            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        [TestCase("abs(2)", "ABS(2)")]
        [TestCase("ABS(-2)", "ABS((-2))")]
        [TestCase("Abs(-1.1)", "ABS((-1.1))")]
        [TestCase("abs(1.1)", "ABS(1.1)")]
        public void TestAbsExpressions(string expr, string expectedQuery)
        {
            Setup(expr);

            CalcParser.ExpressionContext context = this._calcParser.expression();

            QueryVisitor visitor = new QueryVisitor();
            string query = visitor.Visit(context);

            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        [TestCase("sgn(2)", "SGN(2)")]
        [TestCase("SGN(-2)", "SGN((-2))")]
        [TestCase("Sgn(-1.1)", "SGN((-1.1))")]
        [TestCase("sgn(1.1)", "SGN(1.1)")]
        [TestCase("sgn(0)", "SGN(0)")]
        public void TestSngExpressions(string expr, string expectedQuery)
        {
            Setup(expr);

            CalcParser.ExpressionContext context = this._calcParser.expression();

            QueryVisitor visitor = new QueryVisitor();
            string query = visitor.Visit(context);

            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        [TestCase("sqrt(4)", "SQRT(4)")]
        [TestCase("SQRT(1)", "SQRT(1)")]
        [TestCase("Sqrt(0.81)", "SQRT(0.81)")]
        public void TestSrqtExpressions(string expr, string expectedQuery)
        {
            Setup(expr);

            CalcParser.ExpressionContext context = this._calcParser.expression();

            QueryVisitor visitor = new QueryVisitor();
            string query = visitor.Visit(context);

            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        [TestCase("(1 - 2) * (3 + 4)", "(((1-2))*((3+4)))")]
        [TestCase("abs(2 * 2) / sqrt(2 * 2)", "(ABS((2*2))/nullif(SQRT((2*2)), 0))")]
        [TestCase("1 + 2 * 3 / 4", "(1+((2*3)/nullif(4, 0)))")]
        public void TestCompositeArithmeticExpressions(string expr, string expectedQuery)
        {
            Setup(expr);

            CalcParser.ExpressionContext context = this._calcParser.expression();

            QueryVisitor visitor = new QueryVisitor();
            string query = visitor.Visit(context);

            Assert.AreEqual(expectedQuery, query);
        }
    }
}
