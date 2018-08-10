using System;
using DFMServer;
using FieldInfo = DfmServer.Managed.FieldInfo;

namespace Workflow.Expressions.Resolvers
{
    public class BasicMetadataResolver : IMetadataResolver
    {
        private static readonly Lazy<BasicMetadataResolver> Lazy = new Lazy<BasicMetadataResolver>(() => new BasicMetadataResolver());

        public static BasicMetadataResolver Instance => Lazy.Value;

        private BasicMetadataResolver()
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

        public FieldInfo GetField(string fieldName)
        {
            return new FieldInfo { Name = fieldName, Type = DFM_FIELD_TYPE.DFM_FT_STRING };
        }
    }
}
