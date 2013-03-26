using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace QueryMapper
{
    public class QueryMappingVisitor<TFrom, TTo>
        : ExpressionVisitor
    {
        private ScopeStack<IDictionary<ParameterExpression, ParameterExpression>> m_parameterMap = 
            new ScopeStack<IDictionary<ParameterExpression, ParameterExpression>>();
        public IQueryable Query { get; set; }

        public QueryMappingVisitor(IQueryable<TTo> query)
        {
            Query = query;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            foreach (var arg in node.Arguments)
            {
                if (IsFromType(arg.Type))
                {
                    var o = Visit(node.Object);
                    var args = (from v in node.Arguments
                         select Visit(v)).ToArray();

                    SwapFromToType(node.Type);
                    var method = SwapFromToTypeMethod(node.Method, args.Select(a=>a.Type).ToArray());

                    if (o != null) // instance method
                        return Expression.Call(o, method, args);
                    return Expression.Call(method, args);
                }
            }

            return base.VisitMethodCall(node);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            // rewrite lambdas containtin TFrom as a parameter
            if (node.Parameters.Any(p => IsFromType(p.Type)))
            {
                var map = 
                    (from p in node.Parameters
                     select new {previous=p, current=(ParameterExpression)Visit(p)})
                     .ToDictionary(a=>a.previous, a=>a.current);

                using (m_parameterMap.Push(map))
                {
                    return Expression.Lambda(Visit(node.Body), map.Values.ToArray());
                }
            }

            return base.VisitLambda(node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (m_parameterMap.Any())
            {
                var current = m_parameterMap.Peek();
                if (current.ContainsKey(node))
                    return current[node];
            }

            if(IsFromType(node.Type))
                return Expression.Parameter(SwapFromToType(node.Type), node.Name);

            return base.VisitParameter(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            // Only rewrite access for types matching our From paramter
            if(node.Member.ReflectedType == typeof(TFrom))
            {
                // Only operate on properties
                if(node.Member.MemberType == MemberTypes.Property)
                {
                    return GetExpandedPropertyExpression(Visit(node.Expression), node.Member.Name);
                }
                
                if(node.Member.MemberType == MemberTypes.Field)
                {
                    var toField = typeof(TTo).GetField(node.Member.Name);
                    var fromField = (FieldInfo)node.Member;

                    if (toField.FieldType == fromField.FieldType)
                    {
                        return Expression.MakeMemberAccess(Visit(node.Expression), toField);
                    }
                    throw new NotSupportedException("To and From property types must match");
                }
            }

            return base.VisitMember(node);
        }

        private Expression GetExpandedPropertyExpression(Expression o, string name)
        {
            var properties = o.Type.GetProperties();
            foreach (var property in properties)
            {
                if (name.StartsWith(property.Name))
                {
                    var memberExpression = Expression.MakeMemberAccess(o, property);
                    name = name.Remove(0, property.Name.Length);
                    if(name.Length > 0)
                        return GetExpandedPropertyExpression(memberExpression, name);

                    return memberExpression;
                }
            }

            return o;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (typeof(IQueryable<TFrom>).IsAssignableFrom(node.Type))
                return Expression.Constant(Query);

            return base.VisitConstant(node);
        }

        protected bool IsFromType(Type source)
        {
            if (source == typeof(TFrom))
            {
                return true;
            }
            
            if (source.IsGenericType)
            {
                foreach (var arg in source.GetGenericArguments())
                {
                    if (IsFromType(arg))
                        return true;
                }
            }

            return false;
        }

        protected Type SwapFromToType(Type source)
        {
            if (source == typeof(TFrom))
            {
                return typeof(TTo);
            }
            
            if (source.IsGenericType)
            {
                var args = new List<Type>();
                foreach (var arg in source.GetGenericArguments())
                {
                    args.Add(SwapFromToType(arg));
                }

                return source.GetGenericTypeDefinition().MakeGenericType(args.ToArray());
            }

            return source;
        }

        protected MethodInfo SwapFromToTypeMethod(MethodInfo method, params Type[] args)
        {
            if (method.IsGenericMethod)
            {
                var def = method.GetGenericMethodDefinition();
                return def.MakeGenericMethod(
                    (from a in method.GetGenericArguments()
                     select SwapFromToType(a)).ToArray());
            }
            
            if(method.DeclaringType == typeof(TFrom))
            {
                return typeof(TTo).GetMethod(method.Name, args);
            }

            return method;
        }
    }
}
