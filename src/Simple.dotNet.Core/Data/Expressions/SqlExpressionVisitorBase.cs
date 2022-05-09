using Dapper;
using Simple.Core.Dapper;
using Simple.Core.Data.Schema;
using Simple.Core.Expressions;
using Simple.Core.Extensions;
using Simple.Core.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Simple.Core.Data.Expressions
{
    public abstract class SqlExpressionVisitorBase : ExpressionVisitorBase, IDisposable
    {
        protected readonly Stack<string> _where = new Stack<string>();
        protected readonly Stack<string> _not = new Stack<string>();
        protected readonly Stack<bool> _array = new Stack<bool>();
        protected List<SelectProperty> _select = new List<SelectProperty>();
        protected List<MemberExpression> _groupby = new List<MemberExpression>();
        protected List<MemberExpression> _orderby = new List<MemberExpression>();
        protected int skip = 0;
        protected int take = 0;

        private readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();
        private int paramIndex = 0;
        public SqlExpressionVisitorBase(Expression expression)
        {
            base.Visit(expression);
        }

        /// <summary>
        /// 获取sql解析字符串及参数化数据
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string GetCondition(out DynamicParameters parameters)
        {
            parameters = new DynamicParameters();
            string where = string.Concat(_where.ToArray());
            foreach (var item in _parameters)
            {
                parameters.Add(item.Key, item.Value);
            }
            _parameters.Clear();
            _where.Clear();
            return where;
        }
        public string GetSqlText(out DynamicParameters parameters)
        {
            parameters = new DynamicParameters();
            if (this.Type == null) throw new NullReferenceException("泛型类型未指定");
            string[] cmd = ArrayHelper.CreateInstance(4, string.Empty);
            cmd[1] = $"[{this.Type.GetTableName()}]";
            //0 查询字段
            //1 表名
            //2 where条件
            //3 排序、分组条件
            string sql = "SELECT {0} FROM {1} {2} {3}";

            if (_where.Count > 0)
            {
                cmd[2] = "WHERE " + string.Concat(_where.ToArray());
            }
            foreach (var cell in this.Cells)
            {
                if (_select.Count == 0)
                {
                    cmd[0] = string.Join(",", this.Type.GetProperties().Where(c => c.CanRead && c.CanWrite && !c.HasAttribute<NotMappedAttribute>()).Select(c => $"[{c.GetFieldName<ColumnAttribute>()}]"));
                }
                if (_orderby.Count == 0 && take > 0)
                {
                    throw new DapperException("Skip、Take，缺少OrderBY");
                }
                switch (cell)
                {
                    case "OrderByDescending":
                        cmd[3] = string.Format("ORDER BY {0} DESC", string.Join(",", _orderby.Select(c => $"[{c.GetFieldName<ColumnAttribute>()}]")));
                        break;
                    case "OrderBy":
                        cmd[3] = string.Format("ORDER BY {0} ASE", string.Join(",", _orderby.Select(c => $"[{c.GetFieldName<ColumnAttribute>()}]")));
                        break;
                    case "GroupBy":
                        cmd[3] = string.Format("GROUP BY {0}", string.Join(",", _groupby.Select(c => $"[{c.GetFieldName<ColumnAttribute>()}]")));
                        break;
                    case "Select":
                        {
                            List<string> fields = new List<string>();
                            foreach (var item in _select)
                            {
                                switch (item.Method)
                                {
                                    case "Key":
                                        PropertyInfo property = this.Type.GetProperties().FirstOrDefault(c => c.Name == item.Member.Name);
                                        if (property == null)
                                        {
                                            fields.Add($"[{item.Member.GetFieldName<ColumnAttribute>()}]");
                                        }
                                        else
                                        {
                                            fields.Add($"[{property.GetFieldName<ColumnAttribute>()}]");
                                        }
                                        break;
                                    case "Count":
                                        fields.Add($"{item.Method}(0) AS [{item.Method}]");
                                        break;
                                    default:
                                        fields.Add($"{item.Method}([{item.Member.GetFieldName<ColumnAttribute>()}]) AS [{item.Method}]");
                                        break;
                                }
                            }
                            cmd[0] = string.Join(",", fields);
                        }
                        break;
                    case "Skip":
                    case "Take":
                        sql = "SELECT {0} FROM {1} {2} {3} " + $"OFFSET {skip} ROW FETCH NEXT {take} ROW ONLY";
                        break;
                    case "Any":
                        cmd[0] = "0";
                        sql = "SELECT 0 WHERE EXISTS(SELECT {0} FROM {1} {2} {3})";
                        break;
                    case "Count":
                        cmd[0] = "0";
                        sql = "SELECT COUNT({0}) FROM {1} {2} {3}";
                        break;
                    case "FirstOrDefault":
                        sql = "SELECT TOP 1 {0} FROM {1} {2} {3}";
                        break;
                    default:
                        break;
                }
            }
            foreach (var item in _parameters)
            {
                parameters.Add(item.Key, item.Value);
            }
            return string.Format(sql, cmd);
        }

        protected override Expression VisitNew(NewExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.New:
                    _select = new SelectExpressionVisitor().Visit(node).ToList();
                    break;
            }
            return node;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            base.VisitUnary(node);
            switch (node.NodeType)
            {
                case ExpressionType.Not:
                    _not.Push(node.NodeType.ToString());
                    break;
            }
            return node;
        }
        /// <summary>
        /// 方法表达式
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {

            switch (node.NodeType)
            {
                case ExpressionType.Call:
                    foreach (var item in node.Arguments)
                    {
                        this.Visit(item);
                    }
                    if (node.Object != null)
                    {
                        this.VisitMember((MemberExpression)node.Object);
                    }
                    if (typeof(IEntity).IsAssignableFrom(node.Type))
                    {
                        this.Type = node.Type;
                    }
                    break;
            }
            this.Cells.Add(node.Method.Name);
            switch (node.Method.Name)
            {
                case "GroupBy":
                    {
                        using (GroupByExpressionVisitor visitor = new GroupByExpressionVisitor())
                        {
                            _groupby = visitor.Visit(node).ToList();
                            this.Type ??= visitor.Type;
                        }
                    }
                    break;
                case "Take":
                case "Skip":
                    {
                        foreach (var item in node.Arguments)
                        {
                            switch (item.NodeType)
                            {
                                case ExpressionType.Constant:
                                    {
                                        ConstantExpression constant = (ConstantExpression)item;
                                        if (constant.Value == null) break;
                                        if (constant.Value is int)
                                        {
                                            switch (node.Method.Name)
                                            {
                                                case "Take":
                                                    this.take = constant.Value.GetValue<int>();
                                                    break;
                                                case "Skip":
                                                    this.skip = constant.Value.GetValue<int>();
                                                    break;
                                            }
                                        }
                                        else if (item.Type.IsGenericType)
                                        {
                                            this.Type = node.Type.GenericTypeArguments[0];
                                        }
                                    }
                                    break;
                                default:
                                    this.Visit(item);
                                    break;
                            }
                        }
                    }
                    break;
                case "OrderByDescending":
                case "OrderBy":
                    {
                        using (OrderByExpressionVisitory visitory = new OrderByExpressionVisitory())
                        {
                            _orderby = visitory.Visit(node).ToList();
                            this.Type ??= visitory.Type;
                        }
                    }
                    break;
                case "Contains":
                case "StartsWith":
                case "EndsWith":
                    {
                        string field = _where.Pop();
                        string param = _where.Pop();
                        string not = string.Empty;
                        bool array = false;
                        while (_not.Count > 0)
                        {
                            not = _not.Pop();
                        }
                        while (_array.Count > 0)
                        {
                            array = _array.Pop();
                        }
                        if (array)
                        {
                            _where.Push($"{field} {not} IN ({param})");
                        }
                        else
                        {
                            switch (node.Method.Name)
                            {
                                case "Contains":
                                    _where.Push($"{field} {not} LIKE '%{param}%'");
                                    break;
                                case "StartsWith":
                                    _where.Push($"{field} {not} LIKE '%{param}'");
                                    break;
                                case "EndsWith":
                                    _where.Push($"{field} {not} LIKE '{param}%'");
                                    break;
                            }
                        }
                    }
                    break;
                default:
                    foreach (var item in node.Arguments)
                    {
                        this.Visit(item);
                    }
                    break;
            }
            return node;
        }
        protected override Expression VisitConstant(ConstantExpression node)
        {
            base.VisitConstant(node);
            while (_value.Count > 0)
            {
                object value = _value.Pop();
                if (value is Array)
                {
                    _array.Push(true);
                }
                string name = AppendContant(value);
                _where.Push(name);
            }
            return node;
        }
        protected override Expression VisitMember(MemberExpression node)
        {
            base.VisitMember(node);
            while (_value.Count > 0)
            {
                object value = _value.Pop();
                if (value is Array)
                {
                    _array.Push(true);
                }
                string name = AppendContant(value);
                _where.Push(name);
            }
            while (_field.Count > 0)
            {
                Expression field = _field.Pop();
                string fieldname = field.GetFieldName<ColumnAttribute>();
                _where.Push(fieldname);
            }

            return node;
        }
        /// <summary>
        /// 二元表达式
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            this._where.Push(")");
            base.Visit(node.Right);
            this._where.Push($" {node.NodeType.GetExpressionType()} ");
            base.Visit(node.Left);
            this._where.Push("(");
            return node;
        }

        public string AppendContant(object value)
        {
            string name = $"@_p{paramIndex}";
            _parameters.Add(name, value);
            paramIndex++;
            return name;
        }

        public void Dispose()
        {
            _parameters.Clear();
            _where.Clear();
            _value.Clear();
        }
    }
}
