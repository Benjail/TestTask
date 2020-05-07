using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace TestTask.Models
{
    public class Customer : IdentityUser<Guid>
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Address { get; set; }
        public float Discount { get; set; }

        public List<Order> Orders { get; set; }
    }
}
