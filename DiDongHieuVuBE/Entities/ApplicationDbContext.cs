using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using System.Reflection.Emit;
using System;

namespace ManageEmployee.Entities
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Employee>? Employees { get; set; }
        public virtual DbSet<Company>? Companies { get; set; }
        public virtual DbSet<User>? Users { get; set; }
        public virtual DbSet<UserRole>? UserRoles { get; set; }
        public virtual DbSet<Province>? Provinces { get; set; }
        public virtual DbSet<District>? Districts { get; set; }
        public virtual DbSet<Ward>? Wards { get; set; }
        public virtual DbSet<DeskFloor>? DeskFloors { get; set; }
        public virtual DbSet<Job>? Jobs { get; set; }
        public virtual DbSet<Customer>? Customers { get; set; }
        public virtual DbSet<Category>? Categories { get; set; }
        public virtual DbSet<Menu>? Menus { get; set; }
        public virtual DbSet<MenuRole>? MenuRoles { get; set; }
        public virtual DbSet<PagePrint>? PagePrints { get; set; }
        public virtual DbSet<Print>? Prints { get; set; }

        public string RemoveAcents(string @String) => throw new InvalidOperationException();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDbFunction(
                typeof(ApplicationDbContext).GetMethod(nameof(RemoveAcents), new[] { typeof(string) }),
                b =>
                {
                    b.HasName("RemoveAcents");
                    b.HasParameter("String");
                });
        }
    }
}
