using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using AutoMapper;
using FluentAssertions;
using System.Diagnostics;

namespace QueryMapper.Tests.EntityFramework
{
    [TestClass]
    public class EFTests
    {
        public class OrderDto
        {
            public int OrderId { get; set; }
            public DateTime OrderDate { get; set; }
            public string CustomerName { get; set; }
            public int CustomerId { get; set; }
        }

        [TestInitialize]
        public void Initialize()
        {
            var migrator = new System.Data.Entity.Migrations.DbMigrator(new Migrations.Configuration());
            migrator.Update(); 

            Mapper.Reset();
            Mapper.CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id));
        }


        [TestMethod]
        public void EntityFramework_FlattenedQuery_Customer1()
        {
            var context = new SampleDataContext();
            var query = context.Orders.Include(o=>o.Customer).AsAutoMappedQuery<OrderDto, Order>();

            var result = query.Where(o => o.CustomerName == "Customer 1").ToArray();
            result.Count().Should().Be(2);

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result));
        }

        [TestMethod]
        public void EntityFramework_FlattenedQuery_Customer4()
        {
            var context = new SampleDataContext();
            var query = context.Orders.Include(o => o.Customer).AsAutoMappedQuery<OrderDto, Order>();

            var result = query.Where(o => o.CustomerName == "Customer 4").ToArray();
            result.Count().Should().Be(1);

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result));
        }

        [TestMethod]
        public void EntityFramework_FlattenedQuery_Count()
        {
            var context = new SampleDataContext();
            var query = context.Orders.Include(o => o.Customer).AsAutoMappedQuery<OrderDto, Order>();

            var result = query.Where(o => o.CustomerName == "Customer 1");
            result.Count().Should().Be(2);

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result));
        }

        [TestMethod]
        public void EntityFramework_FlattenedQuery_Any()
        {
            var context = new SampleDataContext();
            var query = context.Orders.Include(o => o.Customer).AsAutoMappedQuery<OrderDto, Order>();

            var result = query.Where(o => o.CustomerName == "Customer 4");
            result.Any().Should().BeTrue();

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result));
        }
    }
}
