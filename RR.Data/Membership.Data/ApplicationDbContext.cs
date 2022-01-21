using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RR.Data.Membership.Data
{
     public class ApplicationDbContext : IdentityDbContext
     {
          public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
              : base(options)
          {
          }
     }
}
