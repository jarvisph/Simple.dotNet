using Simple.Core.Extensions;
using Simple.Core.Helper;
using Simple.Core.Test.Expressions;
using System.Reflection;

string action = args.Get("-action", "expressions");
string method = args.Get("-method", "Where");
switch (action)
{
    case "expressions":
        Type type = typeof(ExpressionTest);
        object? obj = Activator.CreateInstance(type);
        if (type.GetMembers().Any(c => c.Name == method))
        {
            type.InvokeMember(method, BindingFlags.InvokeMethod, null, obj, null);
        }
        else
        {
            ConsoleHelper.WriteLine($"{method}不存在", ConsoleColor.Red);
        }
        break;
    default:
        break;
}