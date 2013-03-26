using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutoMapper;
using FluentAssertions;

namespace QueryMapper.Tests
{
    [TestClass]
    public class FlattenedMapFields
    {
        public class OrderDto
        {
            public int OrderId { get; set; }
            public DateTime OrderDate { get; set; }
            public string CustomerName { get; set; }
            public int CustomerId { get; set; }
        }

        public class OrderEntity
        {
            public int Id { get; set; }
            public DateTime OrderDate { get; set; }
            public CustomerEntity Customer { get; set; }
        }

        public class CustomerEntity
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public List<OrderEntity> Orders = new List<OrderEntity>
        {
            new OrderEntity{
                Id = 1, 
                OrderDate = DateTime.Now.AddDays(-10), 
                Customer=new CustomerEntity{
                    Id = 2,
                    Name = "Customer 1"
                }
            },
            new OrderEntity{
                Id = 2, 
                OrderDate = DateTime.Now.AddDays(-10),  
                Customer=new CustomerEntity{
                    Id = 3,
                    Name = "Customer 2"
                }
            },
            new OrderEntity{
                Id = 1, 
                OrderDate = DateTime.Now.AddDays(-10), 
                Customer=new CustomerEntity{
                    Id = 4,
                    Name = "Customer 3"
                }
            },
            new OrderEntity{
                Id = 1, 
                OrderDate = DateTime.Now.AddDays(-10), 
                Customer=new CustomerEntity{
                    Id = 5,
                    Name = "Customer 4"
                }
            },
        };

        [TestMethod]
        public void TestAutomapperFlatteningQuery()
        {
            Mapper.CreateMap<OrderEntity, OrderDto>();
            var query = Orders.AsQueryable().AsMappedQuery(
                new DelegatedMapper<OrderEntity, OrderDto>(t => Mapper.Map(t, new OrderDto())));

            var result = query.Where(o=>o.CustomerName == "Customer 1").ToArray();
            result.Count().Should().Be(1);
        }

        [TestMethod]
        public void TestAutomapperFlatteningQuery2()
        {
            Mapper.CreateMap<OrderEntity, OrderDto>();
            var query = Orders.AsQueryable().AsMappedQuery(
                new DelegatedMapper<OrderEntity, OrderDto>(t => Mapper.Map(t, new OrderDto())));

            var result = query.Where(o => o.CustomerId == 2 || o.CustomerId == 3).ToArray();
            result.Count().Should().Be(2);
        }
    }
}
