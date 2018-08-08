namespace Workflow.Expressions.Resolvers
{
    public interface IDataResolver
    {
        Argument ResolveVariable(string variableName);
        Argument ResolveFieldValue(string fieldName);

        string ResolveField(string fieldName);
    }
}
