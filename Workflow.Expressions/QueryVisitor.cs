﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflow.Expressions
{
    public class QueryVisitor : CalcBaseVisitor<string>
    {
        private readonly IMetadataResolver _resolver;
        public QueryVisitor(IMetadataResolver resolver)
        {
            this._resolver = resolver;
        }

        public QueryVisitor() : this(NullResolver.Instance)
        {
        }

        public override string VisitAdditiveExpression(CalcParser.AdditiveExpressionContext context)
        {
            string arg1 = Visit(context.expression(0));
            string arg2 = Visit(context.expression(1));

            string op = null;
            if (context.Plus() != null)
            {
                op = "+";
            }

            if (context.Minus() != null)
            {
                op = "-";
            }

            if (op == null)
            {
                throw new ArgumentCastException("Unknown additive operation");
            }

            return string.Format($"({arg1}{op}{arg2})") ;
        }

        public override string VisitMultiplicativeExpression(CalcParser.MultiplicativeExpressionContext context)
        {
            string arg1 = Visit(context.expression(0));
            string arg2 = Visit(context.expression(1));

            if (context.Multiply() != null)
            {
                return string.Format($"({arg1}*{arg2})");
            }

            if (context.Divide() != null)
            {
                return string.Format($"({arg1}/nullif({arg2}, 0))");
            }

            throw new ArgumentCastException("Unknown multiplicative operation");
        }

        public override string VisitUnaryMinusExpression(CalcParser.UnaryMinusExpressionContext context)
        {
            string arg = Visit(context.expression());
            return string.Format($"(-{arg})");
        }

        public override string VisitPowerExpression(CalcParser.PowerExpressionContext context)
        {
            string arg1 = Visit(context.expression(0));
            string arg2 = Visit(context.expression(1));

            return string.Format($"({arg1}^{arg2})");
        }

        public override string VisitAbsExpression(CalcParser.AbsExpressionContext context)
        {
            string arg = Visit(context.expression());
            return string.Format($"ABS({arg})");
        }

        public override string VisitSqrtExpression(CalcParser.SqrtExpressionContext context)
        {
            string arg = Visit(context.expression());
            return string.Format($"SQRT({arg})");
        }

        public override string VisitSgnExpression(CalcParser.SgnExpressionContext context)
        {
            string arg = Visit(context.expression());
            return string.Format($"SGN({arg})");
        }

        public override string VisitLiteralExpression(CalcParser.LiteralExpressionContext context)
        {
            return context.GetText();
        }

        public override string VisitParenthesisExpression(CalcParser.ParenthesisExpressionContext context)
        {
            string arg = Visit(context.expression());
            return string.Format($"({arg})");
        }
    }
}