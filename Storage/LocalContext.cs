namespace ASB.Storage
{
    using System.Threading.Tasks;
    using API;
    using Microsoft.EntityFrameworkCore;

    public class LocalContext : DbContext
    {
        /// <summary>
        /// Table dbSet of type <see cref="ServerStatus"/>
        /// </summary>
        public DbSet<ServerStatus> Status { get; set; }

        public async Task<bool> UpdateStatus(ServerStatus status)
        {
            var last = await GetLast();

            if (last == null || last.CreatedAt != status.CreatedAt)
            {
                Status.Add(status);

                return await SaveChangesAsync(true) > 0;
            }
            return false;
        }

        /// <summary>
        /// Last status from db table
        /// </summary>
        public async Task<ServerStatus> GetLast() 
            => await Status.LastOrDefaultAsync();

        /// <summary>
        /// Configuring DB Context on create instance for use sqlite
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlite($"Data Source=asb.db");
    }
}