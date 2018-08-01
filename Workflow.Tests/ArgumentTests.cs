using System;
using NUnit.Framework;
using Workflow.Expressions;

namespace Workflow.Tests
{
    public class ArgumentTests
    {
        public static readonly double Tolerance = .0001;

        [Test]
        public void TestConstructor1()
        {
            string o = "abc";
            Argument arg = new Argument(o);

            Assert.NotNull(arg);
            Assert.AreEqual("abc", arg.Object);
        }


        [Test]
        public void TestConstructor2()
        {
            Argument arg = new Argument(0);

            Assert.NotNull(arg);
            Assert.AreEqual(arg.Object, 0);
        }

        [Test]
        public void TestEquals()
        {
            Argument arg1 = new Argument(0);
            Argument arg2 = new Argument(0);
            Assert.AreEqual(arg1, arg2);

            arg1 = new Argument(1.2);
            arg2 = new Argument(1.2);
            Assert.AreEqual(arg1, arg2);

            arg1 = new Argument(1.2);
            arg2 = new Argument(1.23);
            Assert.AreNotEqual(arg1, arg2);

            arg1 = new Argument("abc");
            arg2 = new Argument("abc");
            Assert.AreEqual(arg1, arg2);

            arg1 = new Argument("abc");
            arg2 = new Argument("def");
            Assert.AreNotEqual(arg1, arg2);

            arg1 = new Argument("abc");
            string s = "abc";
            Assert.AreNotEqual(arg1, s);
        }

        [Test]
        public void TestIsDouble()
        {
            Argument arg = new Argument(0);
            Assert.True(arg.IsDouble);

            arg = new Argument(1.1);
            Assert.True(arg.IsDouble);

            arg = new Argument("abc");
            Assert.False(arg.IsDouble);

            arg = new Argument(new DateTime());
            Assert.False(arg.IsDouble);
        }

        [Test]
        public void TestToBoolean()
        {
            Argument arg = new Argument(true);
            Assert.True(arg.ToBoolean());

            arg = new Argument(false);
            Assert.False(arg.ToBoolean());

            arg = new Argument("abc");
            Assert.Throws<ArgumentCastException>(() => arg.ToBoolean());
        }


        [Test]
        public void TestToDouble()
        {
            Argument arg = new Argument(0);
            Assert.AreEqual(0d, arg.ToDouble(), Tolerance);

            arg = new Argument(1);
            Assert.AreEqual(1d, arg.ToDouble(), Tolerance);

            arg = new Argument(1.1);
            Assert.AreEqual(1.1d, arg.ToDouble(), Tolerance);
            
            arg = new Argument("123.45");
            Assert.AreEqual(123.45d, arg.ToDouble(), Tolerance);

            arg = new Argument("abc");
            Assert.Throws<ArgumentCastException>(() => arg.ToDouble());
        }

        [Test]
        public void TestToInteger()
        {
            Argument arg = new Argument(0);
            Assert.AreEqual(0, arg.ToInteger(), Tolerance);

            arg = new Argument(1);
            Assert.AreEqual(1, arg.ToInteger(), Tolerance);

            arg = new Argument(1.1);
            Assert.AreEqual(1, arg.ToInteger(), Tolerance);

            arg = new Argument("123");
            Assert.AreEqual(123, arg.ToInteger(), Tolerance);

            arg = new Argument("abc");
            Assert.Throws<ArgumentCastException>(() => arg.ToInteger());
        }

        [Test]
        public void TestToString()
        {
            Argument arg = new Argument(0);
            Assert.AreEqual("0", arg.ToString());

            arg = new Argument(1.2);
            Assert.AreEqual("1.2", arg.ToString());

            arg = new Argument("abc");
            Assert.AreEqual("abc", arg.ToString());
        }
    }
}
