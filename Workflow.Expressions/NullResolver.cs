using System;

namespace Workflow.Expressions
{
    public class NullResolver : IMetadataResolver
    {
        private static readonly Lazy<NullResolver> Lazy = new Lazy<NullResolver>(() => new NullResolver());

        public static NullResolver Instance => Lazy.Value;

        private NullResolver()
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
    }
}
