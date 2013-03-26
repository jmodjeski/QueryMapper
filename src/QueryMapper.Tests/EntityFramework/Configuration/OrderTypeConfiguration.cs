using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;

namespace QueryMapper.Tests.EntityFramework.Configuration
{
    public class OrderTypeConfiguration 
        : EntityTypeConfiguration<Order>
    {
        public OrderTypeConfiguration()
        {
            Property(o => o.Id).HasDatabaseGeneratedOption(
                DatabaseGeneratedOption.Identity);
        }
    }
}
