using System;

namespace Workflow.Expressions
{
    public enum ResultType
    {
        Date,
        Number,
        String,
        Undefined
    }

    public class Query
    {
        public Query()
        {
            Text = string.Empty;
            ExpectedResult = ResultType.Undefined;
        }

        public Query(string text, ResultType expectedResult)
        {
            Text = text;
            ExpectedResult = expectedResult;
        }

        public string Text { get; set; }
        public ResultType ExpectedResult { get; set; }

        public override string ToString()
        {
            return Text;
        }

        public static implicit operator string(Query q)
        {
            return q.ToString();
        }
    }
}
