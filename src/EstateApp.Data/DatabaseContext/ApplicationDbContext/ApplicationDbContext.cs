using Microsoft.EntityFrameworkCore;
using EstateApp.Data.Entities;

namespace EstateApp.Data.DatabaseContext.ApplicationDbContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options) 
            : base (options) 
            { 

            }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Contact> Contacts { get; set;}
        

            
    }
            
}
    
