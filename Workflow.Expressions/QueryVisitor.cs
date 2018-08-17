using System;
using DFMServer;
using Workflow.Expressions.Resolvers;
using FieldInfo = DfmServer.Managed.FieldInfo;

namespace Workflow.Expressions
{
    public class QueryVisitor : CalcBaseVisitor<Query>
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

        public override Query VisitAdditiveExpression(CalcParser.AdditiveExpressionContext context)
        {
            Query q1 = Visit(context.expression(0));
            Query q2 = Visit(context.expression(1));

            if (context.Plus() != null)
            {
                ResultType type = ResultType.Undefined;
                if (q1.ExpectedResult == ResultType.Number)
                {
                    if (q2.ExpectedResult == ResultType.Number)
                    {
                        type = ResultType.Number;
                    }
                    else if (q2.ExpectedResult == ResultType.Date)
                    {
                        type = ResultType.Date;
                    }
                }

                if (q1.ExpectedResult == ResultType.Date)
                {
                    if (q2.ExpectedResult == ResultType.Number)
                    {
                        type = ResultType.Date;
                    }
                }

                return new Query($"({q1}+{q2})", type);
            }

            if (context.Minus() != null)
            {
                ResultType type = ResultType.Undefined;
                if (q1.ExpectedResult == ResultType.Number && q2.ExpectedResult == ResultType.Number)
                {
                    type = ResultType.Number;
                }

                if (q1.ExpectedResult == ResultType.Date)
                {
                    if (q2.ExpectedResult == ResultType.Date)
                    {
                        type = ResultType.Number;
                    }
                    else if (q2.ExpectedResult == ResultType.Number)
                    {
                        type = ResultType.Date;
                    }
                }

                string queryText = $"({q1}-{q2})";

                if (q1.ExpectedResult == ResultType.Date && q2.ExpectedResult == ResultType.Date)
                {
                    queryText = this._dbTranslator.CastToInt(queryText);
                }

                return new Query(queryText, type);
            }

            throw new ExpressionException("Unknown additive operation: " + context.GetText());
        }

        public override Query VisitMultiplicativeExpression(CalcParser.MultiplicativeExpressionContext context)
        {
            Query q1 = Visit(context.expression(0));
            Query q2 = Visit(context.expression(1));

            ResultType type = ResultType.Undefined;

            if (q1.ExpectedResult == ResultType.Number && q2.ExpectedResult == ResultType.Number)
            {
                type = ResultType.Number;
            }

            if (context.Multiply() != null)
            {
                return new Query($"({q1}*{q2})", type);
            }

            if (context.Divide() != null)
            {
                return new Query($"({q1}/nullif({q2}, 0))", type);
            }

            throw new ExpressionException("Unknown multiplicative operation: " + context.GetText());
        }

        public override Query VisitUnaryMinusExpression(CalcParser.UnaryMinusExpressionContext context)
        {
            Query q = Visit(context.expression());
            return new Query($"(-{q})", ResultType.Number);
        }

        public override Query VisitPowerExpression(CalcParser.PowerExpressionContext context)
        {
            Query q1 = Visit(context.expression(0));
            Query q2 = Visit(context.expression(1));

            ResultType type = ResultType.Undefined;
            if (q1.ExpectedResult == ResultType.Number && q2.ExpectedResult == ResultType.Number)
            {
                type = ResultType.Number;
            }

            return new Query($"({q1}^{q2})", type);
        }

        public override Query VisitAbsExpression(CalcParser.AbsExpressionContext context)
        {
            Query q = Visit(context.expression());

            ResultType type = ResultType.Undefined;
            if (q.ExpectedResult == ResultType.Number)
            {
                type = ResultType.Number;
            }

            return new Query($"ABS({q})", type);
        }

        public override Query VisitSqrtExpression(CalcParser.SqrtExpressionContext context)
        {
            Query q = Visit(context.expression());

            ResultType type = ResultType.Undefined;
            if (q.ExpectedResult == ResultType.Number)
            {
                type = ResultType.Number;
            }

            return new Query($"SQRT({q})", type);
        }

        public override Query VisitSgnExpression(CalcParser.SgnExpressionContext context)
        {
            Query q = Visit(context.expression());

            ResultType type = ResultType.Undefined;
            if (q.ExpectedResult == ResultType.Number)
            {
                type = ResultType.Number;
            }

            return new Query($"SGN({q})", type);
        }
        
        public override Query VisitParenthesisExpression(CalcParser.ParenthesisExpressionContext context)
        {
            Query q = Visit(context.expression());
            return new Query($"({q})", q.ExpectedResult);
        }

        public override Query VisitUnknownFunctionExpression(CalcParser.UnknownFunctionExpressionContext context)
        {
            throw new ExpressionException("Unknown function: " + context.GetText());
        }

        public override Query VisitFieldExpression(CalcParser.FieldExpressionContext context)
        {
            Query fieldQuery = Visit(context.expression());
            string fieldName = fieldQuery.Text;
            FieldInfo fieldInfo = this._metadataResolver.GetField(fieldName);

            ResultType type = ResultType.Number;
            string queryText;
            if (fieldInfo.Type != DFM_FIELD_TYPE.DFM_FT_STRING && fieldInfo.Type != DFM_FIELD_TYPE.DFM_FT_MEMO)
            {
                if (fieldInfo.Type == DFM_FIELD_TYPE.DFM_FT_DATE)
                {
                    type = ResultType.Date;
                }

                queryText = "X" + fieldInfo.Name;
            }
            else
            {
                queryText = this._dbTranslator.FieldToInt(fieldInfo);
            }

            return new Query(queryText, type);
        }

        public override Query VisitFldlenExpression(CalcParser.FldlenExpressionContext context)
        {
            Query fieldQuery = Visit(context.expression());
            string fieldName = fieldQuery.Text;

            FieldInfo fieldInfo = this._metadataResolver.GetField(fieldName);
            string queryText = this._dbTranslator.FieldLength(fieldInfo);

            return new Query(queryText, ResultType.Number);
        }

        public override Query VisitNormdExpression(CalcParser.NormdExpressionContext context)
        {
            Query fieldQuery = Visit(context.expression());
            string fieldName = fieldQuery.Text;

            FieldInfo fieldInfo = this._metadataResolver.GetField(fieldName);

            string queryText;
            switch (fieldInfo.Type)
            {
                case DFM_FIELD_TYPE.DFM_FT_DATE:
                    queryText = "X" + fieldName;
                    break;
                case DFM_FIELD_TYPE.DFM_FT_MEMO:
                case DFM_FIELD_TYPE.DFM_FT_STRING:
                    queryText = this._dbTranslator.FieldToDate(fieldInfo);
                    break;
                default:
                    throw new ExpressionException("$NORMD can be applied for DATE and STRING fields only");
            }

            return new Query(queryText, ResultType.Date);
        }

        public override Query VisitVarExpression(CalcParser.VarExpressionContext context)
        {
            Query variableQuery = Visit(context.expression());
            string variableName = variableQuery.Text;

            Argument arg = this._metadataResolver.ResolveVariable(variableName);

            ResultType type = ResultType.String;
            if (arg.IsDate)
            {
                type = ResultType.Date;
            }

            if (arg.IsDouble)
            {
                type = ResultType.Number;
            }

            string queryText = TranslateArgument(arg);

            return new Query(queryText, type);
        }

        public override Query VisitConstantExpression(CalcParser.ConstantExpressionContext context)
        {
            Argument arg = null;
            if (context.ConstE() != null)
            {
                arg = new Argument(Math.E);
            }

            if (context.ConstPi() != null)
            {
                arg = new Argument(Math.PI);
            }

            if (arg == null)
            {
                throw new ExpressionException("Unknown constant: " + context.GetText());
            }

            return new Query(arg.ToString(), ResultType.Number);
        }

        public override Query VisitRelationalExpression(CalcParser.RelationalExpressionContext context)
        {
            Query q1 = Visit(context.expression(0));
            Query q2 = Visit(context.expression(1));
            
            if (context.GreaterThan() != null)
            {
                return new Query($"({q1}>{q2})", ResultType.Number);
            }

            if (context.GreaterThanEquals() != null)
            {
                return new Query($"({q1}>={q2})", ResultType.Number);
            }

            if (context.LessThan() != null)
            {
                return new Query($"({q1}<{q2})", ResultType.Number);
            }

            if (context.LessThanEquals() != null)
            {
                return new Query($"({q1}<={q2})", ResultType.Number);
            }

            throw new ExpressionException("Unknown relational expression: " + context.GetText());
        }

        public override Query VisitEqualityExpression(CalcParser.EqualityExpressionContext context)
        {
            Query q1 = Visit(context.expression(0));
            Query q2 = Visit(context.expression(1));

            if (context.Equals_() != null)
            {
                return new Query($"({q1}={q2})", ResultType.Number);
            }

            if (context.NotEquals() != null)
            {
                return new Query($"({q1}<>{q2})", ResultType.Number);
            }

            throw new ExpressionException("Unknown equality expression: " + context.GetText());
        }

        public override Query VisitLiteral(CalcParser.LiteralContext context)
        {
            string text = context.GetText();
            Argument arg;

            ResultType type;
            if (context.DateLiteral() != null)
            {
                DateTime? date = DateUtils.MultiParseDate(text);
                if (!date.HasValue)
                {
                    throw new ExpressionException("Incorrect format for a date literal: " + text);
                }

                arg = new Argument(date.Value);
                type = ResultType.Date;
            }
            else if (context.IntegerLiteral() != null)
            {
                arg = new Argument(int.Parse(text));
                type = ResultType.Number;
            }
            else if (context.DecimalLiteral() != null)
            {
                arg = new Argument(double.Parse(text));
                type = ResultType.Number;
            }
            else
            {
                arg = new Argument(text);
                type = ResultType.String;
            }

            string queryText = TranslateArgument(arg);
            return new Query(queryText, type);
        }

        public override Query VisitIdentifierExpression(CalcParser.IdentifierExpressionContext context)
        {
            string identifier = context.GetText();            
            return new Query(identifier, ResultType.String);
        }

        private string TranslateArgument(Argument arg)
        {
            if (arg.IsDate)
            {
                DateTime date = arg.ToDate().Value;
                return this._dbTranslator.GetDbDate(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
            }

            if (arg.IsTime)
            {
                if (DateUtils.TryMultiParseTime(arg.ToString(), out long seconds))
                {
                    DateTime dt = DateTime.Today.AddSeconds(seconds);
                    return this._dbTranslator.GetToDbTime(dt.Hour, dt.Minute, dt.Second);
                }
            }

            if (arg.IsDouble)
            {
                return arg.ToString(); // fix it
            }

            return arg.ToString();
        } 
    }
}
