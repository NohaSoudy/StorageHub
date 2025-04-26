using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace StorageHub.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public virtual DbSet<FileHub> FileHub { get; set; }
    }
}
