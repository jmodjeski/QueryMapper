using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QueryMapper
{
    public interface IMapper<in TFrom, out TTo>
    {
        TTo Map(TFrom from);
    }

    public class DelegatedMapper<TFrom, TTo>
        : IMapper<TFrom, TTo>
    {
        protected Func<TFrom, TTo> MapFunc { get; set; }

        public DelegatedMapper(Func<TFrom, TTo> map)
        {
            MapFunc = map;
        }

        public TTo Map(TFrom from)
        {
            if (from == null)
                return default(TTo);

            return MapFunc(from);
        }
    }
}
