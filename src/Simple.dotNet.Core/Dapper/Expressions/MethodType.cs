using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.dotNet.Core.Dapper.Expressions
{
    public enum MethodType
    {
        None,
        StartsWith,
        Contains,
        EndsWith,
        OrderByDescending,
        OrderBy,
        Where,
        Select,
        Any,
        Count,
        FirstOrDefault,
        Take,
        Skip
    }
}
