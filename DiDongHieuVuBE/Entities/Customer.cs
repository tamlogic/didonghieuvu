using System;
using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities
{
    public class Customer : BaseEntity
    {
        [Key]
        public long CUSTOMER_ID { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
    }
}