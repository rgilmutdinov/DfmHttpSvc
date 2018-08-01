using System;
using System.Text;

namespace Workflow.Expressions
{
    [Serializable]
    public class ArgumentCastException : Exception
    {
        private static string CreateMessage(string type, Argument argument)
        {
            StringBuilder exceptionMessage = new StringBuilder("Argument can not be interpreted as a ");
            exceptionMessage.Append(type);

            if (argument.IsNull)
            {
                exceptionMessage.Append(".  Arg is null.");
            }
            else
            {
                exceptionMessage.Append(".  Arg Class: ").Append(argument.GetType()).Append(" Value: ").Append(argument);
            }

            return exceptionMessage.ToString();
        }

        public static ArgumentCastException Create(string type, Argument argument)
        {
            string message = CreateMessage(type, argument);
            return new ArgumentCastException(message);
        }

        public ArgumentCastException(string message) : base(message)
        {
        }
    }
}
