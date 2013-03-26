using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace QueryMapper
{
    public static class QueryMapper
    {
        internal static object Configuration { get; set; }

        public static void CreateMapping<TFrom, TTo>()
        {

        }
    }

    public class MapConfiguration
    {
        public List<TypeMapConfiguration> TypeMaps {get; set;}
        
        public MapConfiguration()
        {
            TypeMaps = new List<TypeMapConfiguration>();
        }

        public void Map<TFrom, TTo>()
        {
            var map = new TypeMapConfiguration<TFrom, TTo>();
            TypeMaps.Add(map);
        }

    }

    public class TypeMapConfiguration
    {
        public Type From { get; set; }
        public Type To { get; set; }
        public PropertyMapConfiguration PropertyMaps { get; set; }

        public TypeMapConfiguration()
        {
            PropertyMaps = new PropertyMaps();
        }

        protected void Initialize()
        {
            var properties = From.GetProperties();
            foreach (var property in properties)
            {
                var map = new PropertyMapConfiguration<TFrom, TTo>();
                map.Name = property.Name;
            }
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
                    if (name.Length > 0)
                        return GetExpandedPropertyExpression(memberExpression, name);

                    return memberExpression;
                }
            }

            return o;
        }
    }

    public class TypeMapConfiguration<TFrom, TTo>
        :TypeMapConfiguration
    {
        public TypeMapConfiguration()
        {
            From = typeof(TFrom);
            To = typeof(TTo);
        }



    }

    public class PropertyMapConfiguration<TFrom, TTo>
    {
        public Expression From { get; set; }
        public string Name { get; set; }
    }

}
