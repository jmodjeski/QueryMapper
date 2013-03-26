using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace QueryMapper.Tests
{
    /// <summary>
    /// Summary description for MapPropertyAccessExpression
    /// </summary>
    [TestClass]
    public class MapPropertyAccessExpression
    {
        public class Entity
        {
            public string FieldAccess;

            public string MatchingNameAndType { get; set; }
            public int NonMatchingNameAndType { get; set; }
        }

        public class EntityDto
        {
            public string FieldAccess;

            public string MatchingNameAndType { get; set; }
            public string NonMatchingNameAndType { get; set; }
        }


        [TestMethod]
        public void MapMatchingPropertyNameAndType()
        {
            var sample = new List<Entity>
            {
                new Entity{MatchingNameAndType = "SkipMe"},
                new Entity{MatchingNameAndType = "SkipMeAlso"},
                new Entity{MatchingNameAndType = "Testing"},
                new Entity{MatchingNameAndType = "SkipMe"},
                new Entity{MatchingNameAndType = "Testing"},
                new Entity{MatchingNameAndType = "SkipMe"},
            };

            var visitor = new QueryMappingVisitor<EntityDto, Entity>(sample.AsQueryable());
            Expression<Func<EntityDto, bool>> propertyFilter = e => e.MatchingNameAndType == "Testing";
            var lambda = visitor.Visit(propertyFilter) as LambdaExpression;
            var result = lambda.Compile() as Func<Entity, bool>;
            
            sample.Where(result).Count().Should().Be(2);
        }

        [TestMethod]
        public void MapMatchingFieldNameAndType()
        {
            var sample = new List<Entity>()
            {
                new Entity{FieldAccess = "SkipMe"},
                new Entity{FieldAccess = "SkipMeAlso"},
                new Entity{FieldAccess = "Testing"},
                new Entity{FieldAccess = "SkipMe"},
                new Entity{FieldAccess = "Testing"},
                new Entity{FieldAccess = "SkipMe"},
            };

            var visitor = new QueryMappingVisitor<EntityDto, Entity>(sample.AsQueryable());
            Expression<Func<EntityDto, bool>> fieldFilter = e => e.FieldAccess == "Testing";
            var lambda = visitor.Visit(fieldFilter) as LambdaExpression;
            var result = lambda.Compile() as Func<Entity, bool>;

            sample.Where(result).Count().Should().Be(2);
        }
    }
}
