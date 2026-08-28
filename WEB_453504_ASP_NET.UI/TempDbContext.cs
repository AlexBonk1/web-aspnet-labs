using Microsoft.EntityFrameworkCore;
using WEB_453504_ASP_NET.Domain.Entities;

namespace WEB_453504_ASP_NET.UI
{
    public class TempDbContext : DbContext
    {
        public DbSet<MusicalInstrument> Instruments { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder
        optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite("");
        }
    }
}
