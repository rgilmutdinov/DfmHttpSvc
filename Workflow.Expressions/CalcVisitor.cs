using System;
using System.Diagnostics.CodeAnalysis;
using Antlr4.Runtime.Tree;

namespace Workflow.Expressions
{
    public class CalcVisitor : CalcBaseVisitor<Argument>
    {
        private readonly IMetadataResolver _resolver;
        public CalcVisitor(IMetadataResolver resolver)
        {
            this._resolver = resolver;
        }

        public CalcVisitor() : this(NullResolver.Instance)
        {
        }

        public override Argument VisitAdditiveExpression(CalcParser.AdditiveExpressionContext context)
        {
            Argument arg1 = Visit(context.expression(0));
            Argument arg2 = Visit(context.expression(1));

            if (arg1.IsNull || arg2.IsNull)
            {
                return Argument.Null;
            }

            if (context.Plus() != null)
            {
                return Sum(arg1, arg2);
            }

            if (context.Minus() != null)
            {
                return Difference(arg1, arg2);
            }

            return Argument.Null;
        }

        public override Argument VisitMultiplicativeExpression(CalcParser.MultiplicativeExpressionContext context)
        {
            Argument arg1 = Visit(context.expression(0));
            Argument arg2 = Visit(context.expression(1));

            if (arg1.IsNull || arg2.IsNull)
            {
                return Argument.Null;
            }

            if (context.Multiply() != null)
            {
                return Multiplication(arg1, arg2);
            }

            if (context.Divide() != null)
            {
                return Division(arg1, arg2);
            }

            return Argument.Null;
        }

        public override Argument VisitUnaryMinusExpression(CalcParser.UnaryMinusExpressionContext context)
        {
            Argument arg = Visit(context.expression());

            if (arg.IsNull)
            {
                return Argument.Null;
            }

            if (arg.IsInteger)
            {
                return new Argument(-arg.ToInteger());
            }

            if (arg.IsDouble)
            {
                return new Argument(-arg.ToDouble());
            }

            throw new ArgumentCastException("Wrong argument in unary minus expression");
        }

        public override Argument VisitPowerExpression(CalcParser.PowerExpressionContext context)
        {
            Argument arg1 = Visit(context.expression(0));
            Argument arg2 = Visit(context.expression(1));

            if (arg1.IsNull || arg2.IsNull)
            {
                return Argument.Null;
            }

            if (arg1.IsDouble && arg2.IsDouble)
            {
                double result = Math.Pow(arg1.ToDouble(), arg2.ToDouble());
                return new Argument(result);
            }

            throw new ArgumentCastException("Wrong arguments are used in exponentiation operation");
        }

        public override Argument VisitAbsExpression(CalcParser.AbsExpressionContext context)
        {
            Argument arg = Visit(context.expression());

            if (arg.IsNull)
            {
                return Argument.Null;
            }

            if (arg.IsDouble)
            {
                return new Argument(Math.Abs(arg.ToDouble()));
            }

            throw ArgumentCastException.Create("double", arg);
        }

        public override Argument VisitSqrtExpression(CalcParser.SqrtExpressionContext context)
        {
            Argument arg = Visit(context.expression());

            if (arg.IsNull)
            {
                return Argument.Null;
            }

            if (arg.IsDouble)
            {
                return new Argument(Math.Sqrt(arg.ToDouble()));
            }

            throw ArgumentCastException.Create("double", arg);
        }

        public override Argument VisitSgnExpression(CalcParser.SgnExpressionContext context)
        {
            Argument arg = Visit(context.expression());

            if (arg.IsNull)
            {
                return Argument.Null;
            }

            if (arg.IsDouble)
            {
                return new Argument(Math.Sign(arg.ToDouble()));
            }

            throw ArgumentCastException.Create("double", arg);
        }

        public override Argument VisitLiteralExpression(CalcParser.LiteralExpressionContext context)
        {
            return new Argument(context.GetText());
        }

        public override Argument VisitParenthesisExpression(CalcParser.ParenthesisExpressionContext context)
        {
            return Visit(context.expression());
        }

        public override Argument VisitFieldExpression(CalcParser.FieldExpressionContext context)
        {
            Argument arg = Visit(context.expression());

            if (arg.IsNull)
            {
                return Argument.Null;
            }

            if (arg.IsDouble || arg.IsDate)
            {
                throw new ArgumentCastException("Argument of $FIELD macro should not be a number or a date");
            }

            string fieldName = arg.ToString();
            return this._resolver.ResolveFieldValue(fieldName);
        }

        public override Argument VisitIdentifierExpression(CalcParser.IdentifierExpressionContext context)
        {
            return new Argument(context.GetText());
        }

        public override Argument VisitUnknownFunctionExpression(CalcParser.UnknownFunctionExpressionContext context)
        {
            throw new ArgumentException("Unknown function '" + context.GetText());
        }

        public override Argument VisitFldlenExpression(CalcParser.FldlenExpressionContext context)
        {
            Argument arg = Visit(context.expression());
            if (arg.IsNull)
            {
                return Argument.Null;
            }

            if (arg.IsDouble || arg.IsDate)
            {
                throw new ArgumentCastException("Argument of $FLDLEN macro should not be a number or a date");
            }

            string fieldName = arg.ToString();
            Argument sArg = this._resolver.ResolveFieldValue(fieldName);
            if (sArg.IsNull)
            {
                return new Argument(0);
            }

            int length = sArg.ToString().Length;
            return new Argument(length);
        }

        public override Argument VisitVarExpression(CalcParser.VarExpressionContext context)
        {
            Argument arg = Visit(context.expression());
            if (arg.IsNull)
            {
                return Argument.Null;
            }

            if (arg.IsDouble || arg.IsDate)
            {
                throw new ArgumentCastException("Argument of $VAR macro should not be a number or a date");
            }

            String variableName = arg.ToString();
            return this._resolver.ResolveVariable(variableName);
        }

        [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
        public override Argument VisitNormdExpression(CalcParser.NormdExpressionContext context)
        {
            Argument arg = Visit(context.expression());

            if (arg.IsNull)
            {
                return Argument.Null;
            }

            if (arg.IsDouble || arg.IsInteger)
            {
                throw new ArgumentCastException("Argument of $NORMD macro should not be a number");
            }

            if (arg.IsTime)
            {
                int time = arg.ToTime();
                if (time != -1)
                {
                    throw new ArgumentCastException("Argument of $NORMD macro should not be a number or a time");
                }
            }

            if (arg.IsDate)
            {
                return new Argument(arg.ToDate().Value);
            }

            // argument is not a date. So, it's a field
            return this._resolver.ResolveFieldValue(arg.ToString());
        }

        public override Argument VisitRelationalExpression(CalcParser.RelationalExpressionContext context)
        {
            Argument arg1 = Visit(context.expression(0));
            Argument arg2 = Visit(context.expression(1));

            if (arg1.IsNull || arg2.IsNull)
            {
                return Argument.Null;
            }

            if (context.GreaterThan() != null)
            {
                return arg1.ToDouble() > arg2.ToDouble() ? new Argument(1.0) : new Argument(0.0);
            }

            if (context.GreaterThanEquals() != null)
            {
                return arg1.ToDouble() >= arg2.ToDouble() ? new Argument(1.0) : new Argument(0.0);
            }

            if (context.LessThan() != null)
            {
                return arg1.ToDouble() < arg2.ToDouble() ? new Argument(1.0) : new Argument(0.0);
            }

            if (context.LessThanEquals() != null)
            {
                return arg1.ToDouble() <= arg2.ToDouble() ? new Argument(1.0) : new Argument(0.0);
            }

            throw new ArgumentCastException("Unknown relational operation");
        }

        public override Argument VisitConstantExpression(CalcParser.ConstantExpressionContext context)
        {
            if (context.ConstE() != null)
            {
                return new Argument(Math.E);
            }

            if (context.ConstPi() != null)
            {
                return new Argument(Math.PI);
            }

            throw new ArgumentCastException("Unknown constant: " + context.GetText());
        }

        public override Argument VisitEqualityExpression(CalcParser.EqualityExpressionContext context)
        {
            Argument arg1 = Visit(context.expression(0));
            Argument arg2 = Visit(context.expression(1));

            if (arg1.IsNull || arg2.IsNull)
            {
                return Argument.Null;
            }

            if (context.Equals_() != null)
            {
                return Equals(arg1, arg2) ? new Argument(1.0) : new Argument(0.0);
            }

            if (context.NotEquals() != null)
            {
                return !Equals(arg1, arg2) ? new Argument(1.0) : new Argument(0.0);
            }

            throw new ArgumentCastException("Unknown equality operation");
        }

        public override Argument VisitErrorNode(IErrorNode node)
        {
            throw new ArgumentException("Invalid token '" + node.GetText());
        }

        [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
        private Argument Sum(Argument arg1, Argument arg2)
        {
            if (arg1.IsInteger && arg2.IsInteger)
            {
                return new Argument(arg1.ToInteger() + arg2.ToInteger());
            }

            if (arg1.IsDouble && arg2.IsDouble)
            {
                return new Argument(arg1.ToDouble() + arg2.ToDouble());
            }

            if (arg1.IsDate && arg2.IsInteger)
            {
                DateTime sumDate = arg1.ToDate().Value.AddDays(arg2.ToInteger());
                return new Argument(sumDate);
            }

            if (arg2.IsDate && arg1.IsInteger)
            {
                DateTime sumDate = arg2.ToDate().Value.AddDays(arg1.ToInteger());
                return new Argument(sumDate);
            }

            if (arg1.IsDate && arg2.IsTime)
            {
                DateTime sumDate = arg1.ToDate().Value.AddSeconds(arg2.ToTime());
                return new Argument(sumDate);
            }

            if (arg2.IsDate && arg1.IsTime)
            {
                DateTime sumDate = arg2.ToDate().Value.AddSeconds(arg1.ToTime());
                return new Argument(sumDate);
            }

            string lhs = arg1.ToString();
            string rhs = arg2.ToString();

            return new Argument(lhs + rhs);
        }

        [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
        private Argument Difference(Argument arg1, Argument arg2)
        {
            if (arg1.IsNull || arg2.IsNull)
            {
                return Argument.Null;
            }

            if (arg1.IsInteger && arg2.IsInteger)
            {
                return new Argument(arg1.ToInteger() - arg2.ToInteger());
            }

            if (arg1.IsDouble && arg2.IsDouble)
            {
                return new Argument(arg1.ToDouble() - arg2.ToDouble());
            }

            if (arg1.IsDate && arg2.IsInteger)
            {
                DateTime lhs = arg1.ToDate().Value;
                int rhs = arg2.ToInteger();

                return new Argument(lhs.AddDays(-rhs));
            }

            if (arg1.IsDate && arg2.IsDate)
            {
                DateTime lhs = arg1.ToDate().Value;
                DateTime rhs = arg2.ToDate().Value;

                return new Argument((int) (lhs - rhs).TotalDays);
            }

            if (arg1.IsDate && arg2.IsTime)
            {
                DateTime lhs = arg1.ToDate().Value;
                int subSeconds = arg2.ToTime();

                return new Argument(lhs.AddSeconds(-subSeconds));
            }

            throw new ArgumentCastException("Wrong arguments are used in substract operation");
        }

        [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
        private Argument Multiplication(Argument arg1, Argument arg2)
        {
            if (arg1.IsInteger && arg2.IsInteger)
            {
                return new Argument(arg1.ToInteger() * arg2.ToInteger());
            }

            if (arg1.IsDouble && arg2.IsDouble)
            {
                return new Argument(arg1.ToDouble() * arg2.ToDouble());
            }

            throw new ArgumentCastException("Wrong arguments are used in multiplication operation");
        }

        [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
        private Argument Division(Argument arg1, Argument arg2)
        {
            if (arg1.IsInteger && arg2.IsInteger)
            {
                return new Argument(arg1.ToInteger() / arg2.ToInteger());
            }

            if (arg1.IsDouble && arg2.IsDouble)
            {
                return new Argument(arg1.ToDouble() / arg2.ToDouble());
            }

            throw new ArgumentCastException("Wrong arguments are used in divide operation");
        }
    }
}
