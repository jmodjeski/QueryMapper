using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace QueryMapper
{
    public static class MappedQueryExtensions
    {
        public static IQueryable<TFrom> AsMappedQuery<TFrom, TTo>(this IQueryable<TTo> query, IMapper<TTo, TFrom> mapper)
        {
            return new MappedQuery<TFrom, TTo>(query, mapper);
        }

        public static IQueryable<TFrom> AsMappedQuery<TFrom, TTo>(this IQueryable<TTo> query, Func<TTo, TFrom> map)
        {
            return AsMappedQuery(query, new DelegatedMapper<TTo, TFrom>(map));
        }
    }

    public class MappedQuery<TFrom, TTo>
        : IOrderedQueryable<TFrom>, IQueryProvider
    {
        private IQueryable<TTo> Query { get; set; }
        private IMapper<TTo, TFrom> Mapper { get; set; }

        public MappedQuery(IQueryable<TTo> query, IMapper<TTo, TFrom> mapper)
        {
            Query = query;
            Mapper = mapper;
            Expression = Expression.Constant(new TFrom[0].AsQueryable());
        }

        public MappedQuery(IQueryable<TTo> query, IMapper<TTo, TFrom> mapper, Expression expression)
        {
            Query = query;
            Mapper = mapper;
            Expression = expression;
        }

        public IEnumerator<TFrom> GetEnumerator()
        {
            return Provider.Execute<IEnumerable<TFrom>>(Expression).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Type ElementType
        {
            get { return typeof(TFrom); }
        }

        public Expression Expression
        {
            get;
            private set;
        }

        public IQueryProvider Provider
        {
            get { return this; }
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (typeof(TElement) != typeof(TFrom))
                return new EnumerableQuery<TElement>(expression);

            return (IQueryable<TElement>) new MappedQuery<TFrom, TTo>(Query, Mapper, expression);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return CreateQuery<TFrom>(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult)Execute(expression);
        }

        public object Execute(Expression expression)
        {
            var rewriter = new QueryMappingVisitor<TFrom, TTo>(Query);
            var query = rewriter.Visit(expression);

            while(query.CanReduce)
            {
                query = query.Reduce();
            }

            // Handle enumerated map return type
            // Need to check for interface as actual type may be IQueryable (EF)
            if (query.Type.GetInterface(typeof(IEnumerable<TTo>).Name) != null)
            {
                var result = Expression.Lambda<Func<IEnumerable<TTo>>>(
                    query, (IEnumerable<ParameterExpression>) null).Compile()();

                return result.Select(x => Mapper.Map(x));
            }
            
            // Handle single map return type (First/Single/FirstOrDefault/SingleOrDefault)
            if (query.Type == typeof(TTo))
            {
                var result = Expression.Lambda<Func<TTo>>(
                    query, (IEnumerable<ParameterExpression>) null).Compile()();

                return Mapper.Map(result);
            }

            var lambda = Expression.Lambda(
                query, (IEnumerable<ParameterExpression>) null).Compile();

            return lambda.DynamicInvoke();
        }
    }
}
