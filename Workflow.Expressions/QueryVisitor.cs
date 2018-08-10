using System;
using DFMServer;
using Workflow.Expressions.Resolvers;
using FieldInfo = DfmServer.Managed.FieldInfo;

namespace Workflow.Expressions
{
    public class QueryVisitor : CalcBaseVisitor<string>
    {
        private readonly IMetadataResolver _metadataResolver;
        private readonly DbTranslator _dbTranslator;

        public QueryVisitor(IMetadataResolver metadataResolver, DbTranslator dbTranslator)
        {
            this._metadataResolver = metadataResolver;
            this._dbTranslator = dbTranslator;
        }

        public QueryVisitor() : this(BasicMetadataResolver.Instance, new MssqlTranslator())
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

        public override string VisitUnknownFunctionExpression(CalcParser.UnknownFunctionExpressionContext context)
        {
            throw new ArgumentException("Unknown function '" + context.GetText());
        }

        public override string VisitFieldExpression(CalcParser.FieldExpressionContext context)
        {
            string fieldName = Visit(context.expression());
            FieldInfo fieldInfo = this._metadataResolver.GetField(fieldName);
            if (fieldInfo.Type != DFM_FIELD_TYPE.DFM_FT_STRING && fieldInfo.Type != DFM_FIELD_TYPE.DFM_FT_MEMO)
            {
                return "X" + fieldInfo.Name;
            }
            
            return this._dbTranslator.FieldToInt(fieldInfo);
        }

        public override string VisitFldlenExpression(CalcParser.FldlenExpressionContext context)
        {
            string fieldName = Visit(context.expression());
            FieldInfo fieldInfo = this._metadataResolver.GetField(fieldName);
            return this._dbTranslator.FieldLength(fieldInfo);
        }

        public override string VisitVarExpression(CalcParser.VarExpressionContext context)
        {
            string variableName = Visit(context.expression());
            Argument arg = this._metadataResolver.ResolveVariable(variableName);

            if (arg.IsDate)
            {
                DateTime date = arg.ToDate().Value;
                return this._dbTranslator.GetDbDate(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
            }

            if (arg.IsTime)
            {
                int seconds = arg.ToTime();
                DateTime dt = DateTime.Today.AddSeconds(seconds);
                return this._dbTranslator.GetToDbTime(dt.Hour, dt.Minute, dt.Second);
            }

            return arg.ToString();
        }

        public override string VisitLiteral(CalcParser.LiteralContext context)
        {
            Argument arg = new Argument(context.GetText());
            return arg.ToString();
        }

        public override string VisitIdentifierExpression(CalcParser.IdentifierExpressionContext context)
        {
            return context.GetText();
        }
    }
}
