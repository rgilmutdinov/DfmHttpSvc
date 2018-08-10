namespace Workflow.Expressions.Resolvers
{
    public interface IMetadataResolver
    {
        Argument ResolveVariable(string variableName);
        Argument ResolveFieldValue(string fieldName);
    }
}
