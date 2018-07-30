namespace Workflow.Expressions
{
    public class CalcVisitor : CalcBaseVisitor<int>
    {
        public override int VisitAdditiveExpression(CalcParser.AdditiveExpressionContext context)
        {
            int left  = Visit(context.expression(0));
            int right = Visit(context.expression(1));
            int result = 0;

            if (context.Plus() != null)
                result = left + right;

            if (context.Minus() != null)
                result = left - right;

            return result;
        }

        public override int VisitLiteralExpression(CalcParser.LiteralExpressionContext context)
        {
            return int.Parse(context.GetText());
        }

        public override int VisitParenthesisExpression(CalcParser.ParenthesisExpressionContext context)
        {
            return Visit(context.expression());
        }
    }
}
