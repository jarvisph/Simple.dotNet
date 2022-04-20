using Dapper;
using Simple.Core.Data.Schema;
using Simple.Core.Expressions;
using Simple.Core.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Simple.Core.Data.Expressions
{
    public abstract class SqlExpressionVisitorBase : ExpressionVisitorBase
    {
        protected readonly Stack<string> _where = new Stack<string>();
        protected readonly Stack<string> _not = new Stack<string>();
        protected readonly Stack<bool> _array = new Stack<bool>();
        protected readonly Stack<string> _method = new Stack<string>();
        protected readonly List<string> _select = new List<string>();
        protected readonly List<string> _orderby = new List<string>();
        private readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();
        private int paramIndex = 0;
        public SqlExpressionVisitorBase()
        {

        }
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
        public string GetSqlText(out DynamicParameters parameters, out string method)
        {
            method = string.Empty;
            parameters = new DynamicParameters();
            if (this.Type == null) throw new NullReferenceException("泛型类型未指定");
            string[] cmd = new string[5];
            cmd[1] = this.Type.GetTableName();
            //SELECT AdminID,COUNT(0) FROM cyball_Player  GROUP BY AdminID HAVING AdminID>0 ORDER BY AdminID ASC  
            //0 查询字段
            //1 表名
            //2 where条件
            //3 排序条件
            //4 分组条件
            //5 分组后过滤
            string sql = "SELECT {0} FROM {1} {2} {3} {4} {5}";

            if (_where.Count > 0)
            {
                cmd[2] = "WHERE " + string.Concat(_where.ToArray());
            }
            while (_method.Count > 0)
            {
                switch (_method.Pop())
                {
                    case "OrderByDescending":
                        cmd[3] = string.Format("ORDER BY {0} DESC", string.Join(",", _orderby.Select(c => $"[{c}]")));
                        break;
                    case "OrderBy":
                        cmd[3] = string.Format("ORDER BY {0} ASE", string.Join(",", _orderby.Select(c => $"[{c}]")));
                        break;
                    case "Select":
                        cmd[0] = string.Join(",", _select.Select(c => $"[{c}]"));
                        break;
                    case "Any":
                        cmd[0] = "0";
                        sql = "SELECT 0 WHERE EXISTS(SELECT {0} FROM {1} {2} {3})";
                        break;
                    case "Count":
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
        /// <summary>
        /// 解析匿名对象
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitNew(NewExpression node)
        {
            if (node == null) throw new ArgumentNullException(nameof(NewExpression));
            foreach (Expression expression in node.Arguments)
            {
                this.Visit(expression);
            }
            return node;
        }

        protected override MemberBinding VisitMemberBinding(MemberBinding node)
        {
            return base.VisitMemberBinding(node);
        }
        protected override Expression VisitUnary(UnaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Not:
                    _not.Push(node.NodeType.ToString());
                    break;
            }
            return base.VisitUnary(node);
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
                    if (node.Object != null)
                    {
                        VisitMember((MemberExpression)node.Object);
                    }
                    break;
            }
            foreach (var item in node.Arguments)
            {
                this.Visit(item);
            }
            this._method.Push(node.Method.Name);
            switch (node.Method.Name)
            {
                case "GroupBy":

                    break;
                case "Any":
                case "Count":
                case "FirstOrDefault":
                case "Take":
                case "Skip":

                    break;
                case "Contains":
                case "StartsWith":
                case "EndsWith":
                    {
                        string param = _where.Pop();
                        string field = _where.Pop();
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
                            _where.Push($"{param} {not} IN ({field})");
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
            _method.Clear();
            _value.Clear();
        }
    }
}
