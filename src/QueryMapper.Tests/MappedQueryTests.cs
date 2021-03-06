﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using FluentAssertions;

namespace QueryMapper.Tests
{
    [TestClass]
    public class MappedQueryTests
    {
        public class SampleDto
        {
            public string Property1 { get; set; }
            public string Property2 { get; set; }

            public override string ToString()
            {
                return String.Format("Property1: {0}, Property2: {1}", Property1, Property2);
            }
        }

        public class SampleEntity
        {
            public string Property1 { get; set; }
            public string Property2 { get; set; }
        }

        public IQueryable<SampleEntity> Table = new EnumerableQuery<SampleEntity>(
        new [] {
            new SampleEntity{Property1="ABC", Property2="YZ"},
            new SampleEntity{Property1="DEF", Property2="VWX"},
            new SampleEntity{Property1="GHI", Property2="STU"},
            new SampleEntity{Property1="JKL", Property2="PQR"},
            new SampleEntity{Property1="MNO", Property2="MNO"},
            new SampleEntity{Property1="PQR", Property2="JKL"},
            new SampleEntity{Property1="STU", Property2="GHI"},
            new SampleEntity{Property1="VWX", Property2="DEV"},
            new SampleEntity{Property1="YZ",  Property2="ABC"}
        });

        [TestMethod]
        public void MappedQuery_Where()
        {
            var query = Table.AsQueryable().AsMappedQuery(new DelegatedMapper<SampleEntity, SampleDto>(t => new SampleDto
            {
                Property1 = t.Property1,
                Property2 = t.Property2,
            }));

            query = query.Where(t => t.Property1 == "ABC" || t.Property2 == "ABC");

            query.Count().Should().Be(2);
        }

        [TestMethod]
        public void MappedQuery_Select()
        {
            var query = Table.AsQueryable().AsMappedQuery(new DelegatedMapper<SampleEntity, SampleDto>(t => new SampleDto
            {
                Property1 = t.Property1,
                Property2 = t.Property2,
            }));


            var query2 = query.Where(t => t.Property1 == "ABC" || t.Property2 == "ABC").Select(t => new { x = t.Property1 });

            foreach (var item in query2)
            {
                Debug.WriteLine(item.x);
            }
        }

        [TestMethod]
        public void MappedQuery_Join()
        {
            var query = Table.AsQueryable().AsMappedQuery(new DelegatedMapper<SampleEntity, SampleDto>(t => new SampleDto
            {
                Property1 = t.Property1,
                Property2 = t.Property2,
            }));

            var query2 =
                (from t in query
                 join t2 in query on t.Property2 equals t2.Property1
                 select new { First = t, Second = t2 });

            foreach (var item in query2)
            {
                Debug.WriteLine(String.Format("First({0}) -- Second({1})",  item.First, item.Second));
            }
        }

        [TestMethod]
        public void MappedQuery_Single()
        {
            var query =
                Table.AsQueryable().AsMappedQuery(
                    new DelegatedMapper<SampleEntity, SampleDto>(t => new SampleDto {
                        Property1 = t.Property1,
                        Property2 = t.Property2
                    })).Take(1);

            query.Single().Property1.Should().Be(Table.Take(1).Single().Property1);
            query.Single().Property2.Should().Be(Table.Take(1).Single().Property2);
        }

        [TestMethod]
        public void MappedQuery_SingleOrDefault_WithValue()
        {
            var query =
                Table.AsQueryable().AsMappedQuery(
                    new DelegatedMapper<SampleEntity, SampleDto>(t => new SampleDto
                    {
                        Property1 = t.Property1,
                        Property2 = t.Property2
                    })).Take(1);

            query.SingleOrDefault().Property1.Should().Be(Table.Take(1).Single().Property1);
            query.SingleOrDefault().Property2.Should().Be(Table.Take(1).Single().Property2);
        }

        [TestMethod]
        public void MappedQuery_SingleOrDefault_Null()
        {
            var query = new SampleEntity[0]
                .AsQueryable().AsMappedQuery(
                    new DelegatedMapper<SampleEntity, SampleDto>(t => new SampleDto {
                        Property1 = t.Property1,
                        Property2 = t.Property2
                    }));

            query.SingleOrDefault().Should().Be(null);
        }

        [TestMethod]
        public void MappedQuery_First()
        {
            var query =
                Table.AsQueryable().AsMappedQuery(
                    new DelegatedMapper<SampleEntity, SampleDto>(t => new SampleDto
                    {
                        Property1 = t.Property1,
                        Property2 = t.Property2
                    }));

            query.First().Property1.Should().Be(Table.First().Property1);
            query.First().Property2.Should().Be(Table.First().Property2);
        }

        [TestMethod]
        public void MappedQuery_FirstOrDefault_WithValue()
        {
            var query =
                Table.AsQueryable().AsMappedQuery(
                    new DelegatedMapper<SampleEntity, SampleDto>(t => new SampleDto
                    {
                        Property1 = t.Property1,
                        Property2 = t.Property2
                    }));

            query.FirstOrDefault().Property1.Should().Be(Table.First().Property1);
            query.FirstOrDefault().Property2.Should().Be(Table.First().Property2);
        }

        [TestMethod]
        public void MappedQuery_FirstOrDefault_Null()
        {
            var query = new SampleEntity[0]
                .AsQueryable().AsMappedQuery(
                    new DelegatedMapper<SampleEntity, SampleDto>(t => new SampleDto
                    {
                        Property1 = t.Property1,
                        Property2 = t.Property2
                    }));

            query.FirstOrDefault().Should().Be(null);
        }

        [TestMethod]
        public void MappedQuery_Count()
        {
            var query =
                Table.AsQueryable().AsMappedQuery(
                    new DelegatedMapper<SampleEntity, SampleDto>(t => new SampleDto {
                            Property1 = t.Property1,
                            Property2 = t.Property2
                        }));

            query.Count().Should().Be(Table.Count());
        }

        [TestMethod]
        public void MappedQuery_Any_True()
        {
            var query =
                Table.AsQueryable().AsMappedQuery(
                    new DelegatedMapper<SampleEntity, SampleDto>(t => new SampleDto
                    {
                        Property1 = t.Property1,
                        Property2 = t.Property2
                    }));

            query.Any().Should().BeTrue();
        }

        [TestMethod]
        public void MappedQuery_Any_False()
        {
            var query =
                new SampleEntity[0].AsQueryable().AsMappedQuery(
                    new DelegatedMapper<SampleEntity, SampleDto>(t => new SampleDto
                    {
                        Property1 = t.Property1,
                        Property2 = t.Property2
                    }));

            query.Any().Should().BeFalse();
        }

    }
}
