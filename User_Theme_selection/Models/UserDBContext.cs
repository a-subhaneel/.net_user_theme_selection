namespace User_Theme_selection.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class UserDBContext : DbContext
    {
        public UserDBContext()
            : base("name=UserDBContext")
        {
        }

        public DbSet<UserAccounts> userAccount { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
