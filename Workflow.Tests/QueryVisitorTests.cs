using Antlr4.Runtime;
using DFMServer;
using NSubstitute;
using NUnit.Framework;
using Workflow.Expressions;
using Workflow.Expressions.Resolvers;
using FieldInfo = DfmServer.Managed.FieldInfo;

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

        [Test]
        [TestCase("1 + $FIELD(F1)", "(1+XF1)")]
        [TestCase("$FIELD(F1)+$FIELD(F2)+$FIELD(F3)", "((XF1+XF2)+XF3)")]
        [TestCase("$FIELD(F1) - 3 * $FIELD(F2)", "(XF1-(3*XF2))")]
        public void TestIntFieldExpressionsMssql(string expr, string expectedQuery)
        {
            Setup(expr);

            CalcParser.ExpressionContext context = this._calcParser.expression();

            DbTranslator translator = new MssqlTranslator();
            IMetadataResolver resolver = Substitute.For<IMetadataResolver>();
            
            resolver
                .GetField(Arg.Any<string>())
                .Returns(callinfo => new FieldInfo
                {
                    Name = callinfo.ArgAt<string>(0),
                    Type = DFM_FIELD_TYPE.DFM_FT_INTEGER
                });

            QueryVisitor visitor = new QueryVisitor(resolver, translator);

            string query = visitor.Visit(context);
            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        [TestCase("1 + $FIELD(F1)", "(1+CAST(XF1 AS INT))")]
        [TestCase("$FIELD(F1)+$FIELD(F2)+$FIELD(F3)", "((CAST(XF1 AS INT)+CAST(XF2 AS INT))+CAST(XF3 AS INT))")]
        [TestCase("$FIELD(F1) - 3 * $FIELD(F2)", "(CAST(XF1 AS INT)-(3*CAST(XF2 AS INT)))")]
        public void TestStringFieldExpressionsMssql(string expr, string expectedQuery)
        {
            Setup(expr);

            CalcParser.ExpressionContext context = this._calcParser.expression();

            DbTranslator translator = new MssqlTranslator();

            IMetadataResolver resolver = Substitute.For<IMetadataResolver>();

            resolver
                .GetField(Arg.Any<string>())
                .Returns(callinfo => new FieldInfo
                {
                    Name = callinfo.ArgAt<string>(0),
                    Type = DFM_FIELD_TYPE.DFM_FT_STRING
                });

            QueryVisitor visitor = new QueryVisitor(resolver, translator);

            string query = visitor.Visit(context);
            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        [TestCase("1 + $FIELD(F1)", "(1+CAST(XF1 AS INTEGER))")]
        [TestCase("$FIELD(F1)+$FIELD(F2)+$FIELD(F3)", "((CAST(XF1 AS INTEGER)+CAST(XF2 AS INTEGER))+CAST(XF3 AS INTEGER))")]
        [TestCase("$FIELD(F1) - 3 * $FIELD(F2)", "(CAST(XF1 AS INTEGER)-(3*CAST(XF2 AS INTEGER)))")]
        public void TestStringFieldExpressionsFirebird(string expr, string expectedQuery)
        {
            Setup(expr);

            CalcParser.ExpressionContext context = this._calcParser.expression();

            DbTranslator translator = new FirebirdTranslator();

            IMetadataResolver resolver = Substitute.For<IMetadataResolver>();

            resolver
                .GetField(Arg.Any<string>())
                .Returns(callinfo => new FieldInfo
                {
                    Name = callinfo.ArgAt<string>(0),
                    Type = DFM_FIELD_TYPE.DFM_FT_STRING
                });

            QueryVisitor visitor = new QueryVisitor(resolver, translator);

            string query = visitor.Visit(context);
            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        [TestCase("1 + $FIELD(F1)", "(1+TO_NUMBER(XF1))")]
        [TestCase("$FIELD(F1)+$FIELD(F2)+$FIELD(F3)", "((TO_NUMBER(XF1)+TO_NUMBER(XF2))+TO_NUMBER(XF3))")]
        [TestCase("$FIELD(F1) - 3 * $FIELD(F2)", "(TO_NUMBER(XF1)-(3*TO_NUMBER(XF2)))")]
        public void TestStringFieldExpressionsOracle(string expr, string expectedQuery)
        {
            Setup(expr);

            CalcParser.ExpressionContext context = this._calcParser.expression();

            DbTranslator translator = new OracleTranslator();
            IMetadataResolver resolver = Substitute.For<IMetadataResolver>();

            resolver
                .GetField(Arg.Any<string>())
                .Returns(callinfo => new FieldInfo
                {
                    Name = callinfo.ArgAt<string>(0),
                    Type = DFM_FIELD_TYPE.DFM_FT_STRING
                });

            QueryVisitor visitor = new QueryVisitor(resolver, translator);

            string query = visitor.Visit(context);
            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        [TestCase("$FLDLEN(F1) - 3 * $FLDLEN(F2)", "(LEN(XF1)-(3*LEN(XF2)))")]
        public void TestStringFldlenExpressionsMssql(string expr, string expectedQuery)
        {
            Setup(expr);

            CalcParser.ExpressionContext context = this._calcParser.expression();

            DbTranslator translator = new MssqlTranslator();
            IMetadataResolver resolver = Substitute.For<IMetadataResolver>();

            resolver
                .GetField(Arg.Any<string>())
                .Returns(callinfo => new FieldInfo
                {
                    Name = callinfo.ArgAt<string>(0),
                    Type = DFM_FIELD_TYPE.DFM_FT_STRING
                });

            QueryVisitor visitor = new QueryVisitor(resolver, translator);

            string query = visitor.Visit(context);
            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        [TestCase("$FLDLEN(F1) - 3 * $FLDLEN(F2)", "(LENGTH(XF1)-(3*LENGTH(XF2)))")]
        public void TestStringFldlenExpressionsOracle(string expr, string expectedQuery)
        {
            Setup(expr);

            CalcParser.ExpressionContext context = this._calcParser.expression();

            DbTranslator translator = new OracleTranslator();
            IMetadataResolver resolver = Substitute.For<IMetadataResolver>();

            resolver
                .GetField(Arg.Any<string>())
                .Returns(callinfo => new FieldInfo
                {
                    Name = callinfo.ArgAt<string>(0),
                    Type = DFM_FIELD_TYPE.DFM_FT_STRING
                });

            QueryVisitor visitor = new QueryVisitor(resolver, translator);

            string query = visitor.Visit(context);
            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        [TestCase("$FLDLEN(F1) - 3 * $FLDLEN(F2)", "(CHAR_LENGTH(XF1)-(3*CHAR_LENGTH(XF2)))")]
        public void TestStringFldlenExpressionsFirebird(string expr, string expectedQuery)
        {
            Setup(expr);

            CalcParser.ExpressionContext context = this._calcParser.expression();

            DbTranslator translator = new FirebirdTranslator();
            IMetadataResolver resolver = Substitute.For<IMetadataResolver>();

            resolver
                .GetField(Arg.Any<string>())
                .Returns(callinfo => new FieldInfo
                {
                    Name = callinfo.ArgAt<string>(0),
                    Type = DFM_FIELD_TYPE.DFM_FT_STRING
                });

            QueryVisitor visitor = new QueryVisitor(resolver, translator);

            string query = visitor.Visit(context);
            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        [TestCase("$VAR(SYSTIME)", "CONVERT(datetime, '2013-10-10T00:00:00', 126)")]
        [TestCase("1 + $VAR(SYSTIME)+1", "((1+CONVERT(datetime, '2013-10-10T00:00:00', 126))+1)")]
        public void TestQueryVariablesMssql(string expr, string expectedQuery)
        {
            Setup(expr);

            CalcParser.ExpressionContext context = this._calcParser.expression();
            DbTranslator translator = new MssqlTranslator();

            QueryVisitor visitor = new QueryVisitor(new TestMetadataResolver(), translator);
            string query = visitor.Visit(context);

            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        [TestCase("$VAR(SYSTIME)", "CAST('2013-10-10 00:00:00' AS DATE)")]
        [TestCase("1 + $VAR(SYSTIME)+1", "((1+CAST('2013-10-10 00:00:00' AS DATE))+1)")]
        public void TestQueryVariablesFirebird(string expr, string expectedQuery)
        {
            Setup(expr);

            CalcParser.ExpressionContext context = this._calcParser.expression();
            DbTranslator translator = new FirebirdTranslator();

            QueryVisitor visitor = new QueryVisitor(new TestMetadataResolver(), translator);
            string query = visitor.Visit(context);

            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        [TestCase("$VAR(SYSTIME)", "TO_DATE('2013-10-10T00:00:00', 'YYYY-MM-DD\"T\"HH24:MI:SS')")]
        [TestCase("1 + $VAR(SYSTIME)+1", "((1+TO_DATE('2013-10-10T00:00:00', 'YYYY-MM-DD\"T\"HH24:MI:SS'))+1)")]
        public void TestQueryVariablesOracle(string expr, string expectedQuery)
        {
            Setup(expr);

            CalcParser.ExpressionContext context = this._calcParser.expression();
            DbTranslator translator = new OracleTranslator();

            QueryVisitor visitor = new QueryVisitor(new TestMetadataResolver(), translator);
            string query = visitor.Visit(context);

            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        [TestCase("[2013-10-10]", "CONVERT(datetime, '2013-10-10T00:00:00', 126)")]
        [TestCase("[2013-10-10]+1", "(CONVERT(datetime, '2013-10-10T00:00:00', 126)+1)")]
        public void TestQueryDatesMssql(string expr, string expectedQuery)
        {
            Setup(expr);

            CalcParser.ExpressionContext context = this._calcParser.expression();
            DbTranslator translator = new MssqlTranslator();
            IMetadataResolver resolver = Substitute.For<IMetadataResolver>();

            QueryVisitor visitor = new QueryVisitor(resolver, translator);
            string query = visitor.Visit(context);

            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        [TestCase("[2013-10-10]", "TO_DATE('2013-10-10T00:00:00', 'YYYY-MM-DD\"T\"HH24:MI:SS')")]
        [TestCase("[2013-10-10]+1", "(TO_DATE('2013-10-10T00:00:00', 'YYYY-MM-DD\"T\"HH24:MI:SS')+1)")]
        public void TestQueryDatesOracle(string expr, string expectedQuery)
        {
            Setup(expr);

            CalcParser.ExpressionContext context = this._calcParser.expression();
            DbTranslator translator = new OracleTranslator();
            IMetadataResolver resolver = Substitute.For<IMetadataResolver>();

            QueryVisitor visitor = new QueryVisitor(resolver, translator);
            string query = visitor.Visit(context);

            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        [TestCase("[2013-10-10]", "CAST('2013-10-10 00:00:00' AS DATE)")]
        [TestCase("[2013-10-10]+1", "(CAST('2013-10-10 00:00:00' AS DATE)+1)")]
        [TestCase("1+[2013-10-10]+1", "((1+CAST('2013-10-10 00:00:00' AS DATE))+1)")]
        public void TestQueryDatesFirebird(string expr, string expectedQuery)
        {
            Setup(expr);

            CalcParser.ExpressionContext context = this._calcParser.expression();
            DbTranslator translator = new FirebirdTranslator();
            IMetadataResolver resolver = Substitute.For<IMetadataResolver>();

            QueryVisitor visitor = new QueryVisitor(resolver, translator);
            string query = visitor.Visit(context);

            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        [TestCase("$NORMD(STRFLD1)", "CONVERT(datetime, XSTRFLD1, 126)")]
        [TestCase("[2013-10-10]-$NORMD(DATSTRFLD)", "CAST((CONVERT(datetime, '2013-10-10T00:00:00', 126)-CONVERT(datetime, XDATSTRFLD, 126)) as INT)")]
        public void TestNormdMssql(string expr, string expectedQuery)
        {
            Setup(expr);

            CalcParser.ExpressionContext context = this._calcParser.expression();
            DbTranslator translator = new MssqlTranslator();
            IMetadataResolver resolver = new TestMetadataResolver();

            QueryVisitor visitor = new QueryVisitor(resolver, translator);
            string query = visitor.Visit(context);

            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        [TestCase("$NORMD(STRFLD1)", "TO_DATE(XSTRFLD1, 'YYYY-MM-DD\"T\"HH24:MI:SS')")]
        [TestCase("[2013-10-10]-$NORMD(DATSTRFLD)", "FLOOR((TO_DATE('2013-10-10T00:00:00', 'YYYY-MM-DD\"T\"HH24:MI:SS')-TO_DATE(XDATSTRFLD, 'YYYY-MM-DD\"T\"HH24:MI:SS')))")]
        public void TestNormdOracle(string expr, string expectedQuery)
        {
            Setup(expr);

            CalcParser.ExpressionContext context = this._calcParser.expression();
            DbTranslator translator = new OracleTranslator();
            IMetadataResolver resolver = new TestMetadataResolver();

            QueryVisitor visitor = new QueryVisitor(resolver, translator);
            string query = visitor.Visit(context);

            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        [TestCase("$NORMD(STRFLD1)", "CAST(REPLACE(XSTRFLD1, 'T', ' ') AS DATE)")]
        [TestCase("[2013-10-10]-$NORMD(DATSTRFLD)", "CAST((CAST('2013-10-10 00:00:00' AS DATE)-CAST(REPLACE(XDATSTRFLD, 'T', ' ') AS DATE)) as INTEGER)")]
        public void TestNormdFirebird(string expr, string expectedQuery)
        {
            Setup(expr);

            CalcParser.ExpressionContext context = this._calcParser.expression();
            DbTranslator translator = new FirebirdTranslator();
            IMetadataResolver resolver = new TestMetadataResolver();

            QueryVisitor visitor = new QueryVisitor(resolver, translator);
            string query = visitor.Visit(context);

            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        [TestCase("PI + E", "(3.14159265358979+2.71828182845905)")]
        public void TestMathConstants(string expr, string expectedQuery)
        {
            Setup(expr);

            CalcParser.ExpressionContext context = this._calcParser.expression();
            DbTranslator translator = Substitute.For<DbTranslator>();
            IMetadataResolver resolver = Substitute.For<IMetadataResolver>();

            QueryVisitor visitor = new QueryVisitor(resolver, translator);
            string query = visitor.Visit(context);

            Assert.AreEqual(expectedQuery, query);
        }
    }
}
