namespace EstateApp.Data.DatabaseContext
{
    public class AuthenticationDbContext : IdentityDbContext
    {
        public AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options) 
            : base(options)
        {

        }
    }
}