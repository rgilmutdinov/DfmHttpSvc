using DfmServer.Managed;

namespace Workflow.Expressions.Resolvers
{
    public interface IMetadataResolver
    {
        Argument ResolveVariable(string variableName);
        Argument ResolveFieldValue(string fieldName);

        FieldInfo GetField(string fieldName);
    }
}
