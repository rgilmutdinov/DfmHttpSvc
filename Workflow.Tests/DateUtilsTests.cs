using System;
using NUnit.Framework;
using Workflow.Expressions;

namespace Workflow.Tests
{
    public class DateUtilsTests
    {
        [Test]
        [TestCase("2018/05/22")]
        [TestCase("2018-05-22")]
        [TestCase("22-05-2018")]
        [TestCase("22/05/2018")]
        public void TestDateParse(string input)
        {
            bool ok = DateUtils.TryMultiParseDate(input, out DateTime dateTime);

            Assert.AreEqual(true, ok);
            Assert.AreEqual(new DateTime(2018, 05, 22), dateTime);
        }

        [Test]
        [TestCase("2018/05/22T13:16:52")]
        [TestCase("2018-05-22T13:16:52")]
        [TestCase("22-05-2018T13:16:52")]
        [TestCase("22/05/2018T13:16:52")]
        public void TestDateTimeParse(string input)
        {
            bool ok = DateUtils.TryMultiParseDate(input, out DateTime dateTime);

            Assert.AreEqual(true, ok);
            Assert.AreEqual(new DateTime(2018, 05, 22, 13, 16, 52), dateTime);
        }

        [Test]
        public void TestTimeParse()
        {
            bool ok = DateUtils.TryMultiParseTime("13:16:52", out long totalSeconds);

            Assert.AreEqual(true, ok);
            Assert.AreEqual(47812, totalSeconds);
        }
    }
}
