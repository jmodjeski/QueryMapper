using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QueryMapper.Tests
{
    public static class AutoMappedQueryExtensions
    {
        public static IQueryable<TFrom> AsAutoMappedQuery<TFrom, TTo>(this IQueryable<TTo> query)
        {
            return query.AsMappedQuery(new DelegatedMapper<TTo,TFrom>(AutoMapper.Mapper.Map<TTo, TFrom>));
        }
    }
}
