namespace QueryMapper.Tests.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using QueryMapper.Tests.EntityFramework;

    internal sealed class Configuration : DbMigrationsConfiguration<QueryMapper.Tests.EntityFramework.SampleDataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(QueryMapper.Tests.EntityFramework.SampleDataContext context)
        {
            var c1 = new Customer
            {
                Name = "Customer 1"
            };

            var c2 =
                new Customer
                {
                    Name = "Customer 2"
                };

            var c3 =
                new Customer
                {
                    Name = "Customer 3"
                };

            var c4 =
                new Customer
                {
                    Name = "Customer 4"
                };

            context.Customers.AddOrUpdate((c) => c.Name, c1, c2, c3, c4);

            c1 = context.Customers.Where(c => c.Name == "Customer 1").ToArray().DefaultIfEmpty(c1).FirstOrDefault();
            c2 = context.Customers.Where(c => c.Name == "Customer 2").ToArray().DefaultIfEmpty(c2).FirstOrDefault();
            c3 = context.Customers.Where(c => c.Name == "Customer 3").ToArray().DefaultIfEmpty(c3).FirstOrDefault();
            c4 = context.Customers.Where(c => c.Name == "Customer 4").ToArray().DefaultIfEmpty(c4).FirstOrDefault();


            context.Orders.AddOrUpdate(o => o.UniqueId,
                new Order
                {
                    UniqueId = Guid.Parse("{6771BFF4-3783-493C-82C0-EF86E1F9C0F0}"),
                    OrderDate = new DateTime(2012, 1, 1),
                    Customer = c1,
                },
                new Order
                {
                    UniqueId = Guid.Parse("{ED5DC9DC-EA7F-4523-B6FD-14993B5FD7D1}"),
                    OrderDate = new DateTime(2012, 1, 2),
                    Customer = c2,
                },
                new Order
                {
                    UniqueId = Guid.Parse("{F29CA1F0-C095-4780-B042-C8D937BED6AC}"),
                    OrderDate = new DateTime(2012, 1, 3),
                    Customer = c3,
                },
                new Order
                {
                    UniqueId = Guid.Parse("{2DCE602A-99DB-43FD-B598-5C0BCB1F2C60}"),
                    OrderDate = new DateTime(2012, 1, 4),
                    Customer = c4,
                },
                new Order
                {
                    UniqueId = Guid.Parse("{2DCE602A-99DB-43FD-B598-5C0BCB1F2C61}"),
                    OrderDate = new DateTime(2012, 1, 1),
                    Customer = c1,
                },
                new Order
                {
                    UniqueId = Guid.Parse("{7D13665A-B277-441D-8482-91CD5064540E}"),
                    OrderDate = new DateTime(2012, 2, 2),
                    Customer = c2,
                },
                new Order
                {
                    UniqueId = Guid.Parse("{0C37AC13-7994-4FB8-8D10-4FAD28159C2A}"),
                    OrderDate = new DateTime(2012, 3, 3),
                    Customer = c3,
                });
            base.Seed(context);
        }
    }
}
