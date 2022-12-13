using DataLayer.Model;
using Microsoft.EntityFrameworkCore;

namespace DataLayer
{
    public class LotteryDbContext : DbContext
    {
        public LotteryDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<GameDraw>? GameDraws { get; set; }
        public DbSet<GameSession>? GameSessions { get; set; }
    }
}