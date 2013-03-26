using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using QueryMapper.Tests.EntityFramework.Configuration;

namespace QueryMapper.Tests.EntityFramework
{
    public class SampleDataContext
        : DbContext
    {
        public IDbSet<Order> Orders { get; set; }
        public IDbSet<Customer> Customers { get; set; }

        public SampleDataContext()
            : base("QueryMapSample")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new OrderTypeConfiguration());
            modelBuilder.Configurations.Add(new CustomerTypeConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }

    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public Customer Customer { get; set; }
        public Guid UniqueId { get; set; }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
