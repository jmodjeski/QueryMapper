using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QueryMapper
{
    public interface IMapper<in TFrom, TTo>
    {
        IEnumerable<TTo> Map(IEnumerable<TFrom> from);
    }

    public class DelegatedMapper<TFrom, TTo>
        : IMapper<TFrom, TTo>
    {
        protected Func<TFrom, TTo> MapFunc { get; set; }

        public DelegatedMapper(Func<TFrom, TTo> map)
        {
            MapFunc = map;
        }

        public IEnumerable<TTo> Map(IEnumerable<TFrom> enumerable)
        {
            foreach(var f in enumerable)
            {
                yield return MapFunc(f);
            }
        }
    }
}
