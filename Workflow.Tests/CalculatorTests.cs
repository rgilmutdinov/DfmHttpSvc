using System;
using NUnit.Framework;
using Workflow.Expressions;

namespace Workflow.Tests
{
    public class CalculatorTests
    {
        [Test]
        [TestCase("$FIELD(INTFLD2)", 2)]
        [TestCase("$FIELD(INTFLD2) + $FIELD(INTFLD1)", 3)]
        [TestCase("$FIELD(INTFLD2) - $FIELD(INTFLD1)", 1)]
        [TestCase("$FIELD(INTFLD2) * $FIELD(INTFLD1)", 2)]
        [TestCase("$FIELD(INTFLD2) / $FIELD(INTFLD1)", 2)]
        [TestCase("abs(-$FIELD(INTFLD2))", 2)]
        [TestCase("sqrt($FIELD(INTFLD1))", 1)]
        [TestCase("sgn(-$FIELD(INTFLD2))", -1)]
        [TestCase("$FIELD(INTFLD2) + $FIELD(INTFLD2) * $FIELD(INTFLD2)", 6)]
        [TestCase("-$FIELD(INTFLD2)", -2)]
        [TestCase("-(-$FIELD(INTFLD2))", 2)]
        [TestCase("($FIELD(DATFLD1) - $FIELD(DATFLD2))*([2013-10-11] - [2013-10-09])", 296)]
        [TestCase("$FIELD(DATFLD1) - $FIELD(DATFLD2)", 148)]
        [TestCase("$FLDLEN(STRFLD1) + $FLDLEN(STRFLD2) + $FIELD(INTFLD1)", 9)]
        [TestCase("$VAR(SYSTIME) - $FIELD(DATFLD1)", 5)]
        [TestCase("$VAR(SYSTIME_YMD)-$NORMD(DATSTRFLD)", -844)]
        [TestCase("$VAR(SYSTIME_YMD)-$VAR(SYSTIME_YMD)", 0)]
        [TestCase("$NORMD(DATSTRFLD)-$NORMD(DATSTRFLD)", 0)]
        public void TestCalculatorWithIntegerResult(string expression, int expectedResult)
        {
            TestMetadataResolver metadataResolver = new TestMetadataResolver();
            Calculator calc = new Calculator(metadataResolver);

            Argument result = calc.Calculate(expression);
            Assert.True(result.IsInteger);
            Assert.AreEqual(expectedResult, result.ToInteger());
        }

        [Test]
        [TestCase("1.0 + $FIELD(DBLFLD1)", 2.5)]
        [TestCase("1.0 - $FIELD(DBLFLD1)", -0.5)]
        [TestCase("1.0 * $FIELD(DBLFLD1)", 1.5)]
        [TestCase("6.0 / $FIELD(DBLFLD1)", 4.0)]
        public void TestCalculatorWithDoubleResult(string expression, double expectedResult)
        {
            TestMetadataResolver metadataResolver = new TestMetadataResolver();
            Calculator calc = new Calculator(metadataResolver);

            Argument result = calc.Calculate(expression);
            Assert.True(result.IsDouble);
            Assert.AreEqual(expectedResult, result.ToDouble(), 1e-10);
        }

        [Test]
        [TestCase("$FIELD(DATFLD2)", "2013-05-10")]
        [TestCase("$FIELD(DATFLD1)", "2013-10-05")]
        [TestCase("$FIELD(DATFLD1) + 10", "2013-10-15")]
        [TestCase("$FIELD(DATFLD1) - 10", "2013-09-25")]
        [TestCase("$FIELD(DATFLD1) + 1 - 1", "2013-10-05")]
        [TestCase("$FIELD(DATFLD1) - $FIELD(DATFLD2) - 147 + [2013-10-09]", "2013-10-10")]
        [TestCase("$NORMD(DATSTRFLD)", "2016-02-02")]
        public void TestCalculatorWithDateResult(string expression, string expectedResult)
        {
            TestMetadataResolver metadataResolver = new TestMetadataResolver();
            Calculator calc = new Calculator(metadataResolver);

            Argument result = calc.Calculate(expression);
            Assert.True(result.IsDate);
            Assert.AreEqual(expectedResult, result.ToDate().Value.ToString("yyyy-MM-dd"));
        }

        [Test]
        public void TestReservedKeywordsWrongUsage()
        {
            TestMetadataResolver metadataResolver = new TestMetadataResolver();
            string expression = "VAR(SYSTIME_YMD)-$FIELD(DATFLD1)";
            Calculator calc = new Calculator(metadataResolver);

            Assert.Throws<ExpressionException>(() => calc.Calculate(expression));
        }
    }
}
