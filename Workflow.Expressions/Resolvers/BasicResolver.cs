using System;

namespace Workflow.Expressions.Resolvers
{
    public class BasicResolver : IDataResolver
    {
        private static readonly Lazy<BasicResolver> Lazy = new Lazy<BasicResolver>(() => new BasicResolver());

        public static BasicResolver Instance => Lazy.Value;

        private BasicResolver()
        {
        }

        public Argument ResolveVariable(string variableName)
        {
            return Argument.Null;
        }

        public Argument ResolveFieldValue(string fieldName)
        {
            return Argument.Null;
        }

        public string ResolveField(string fieldName)
        {
            return "X" + fieldName;
        }
    }
}
