namespace Workflow.Expressions
{
    public interface IMetadataResolver
    {
        Argument ResolveVariable(string variableName);
        Argument ResolveFieldValue(string fieldName);
    }
}
