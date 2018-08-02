using Antlr4.Runtime;

namespace Workflow.Expressions
{
    public class Calculator
    {
        private readonly CalcVisitor _visitor;

        public Calculator(IMetadataResolver resolver)
        {
            this.MetadataResolver = resolver;
            this._visitor = new CalcVisitor(resolver);
        }

        public IMetadataResolver MetadataResolver { get; set; }

        public Argument Calculate(string expression)
        {
            AntlrInputStream inputStream = new AntlrInputStream(expression);
            CalcLexer calcLexer = new CalcLexer(inputStream);
            calcLexer.RemoveErrorListeners(); //removes the default console listener
            calcLexer.AddErrorListener(new ThrowExceptionErrorListener());

            CommonTokenStream commonTokenStream = new CommonTokenStream(calcLexer);
            CalcParser parser = new CalcParser(commonTokenStream);

            parser.RemoveErrorListeners(); //removes the default console listener
            parser.AddErrorListener(new ThrowExceptionErrorListener());

            CalcParser.ExpressionContext context = parser.expression();

            return this._visitor.Visit(context);
        }
    }
}
