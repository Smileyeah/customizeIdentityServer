using IoTCenter.Data;
using Microsoft.EntityFrameworkCore;

namespace Ganweisoft.IoTCenter.Module.IdentityServer
{
    public class IdentityServerDbContext : GWDbContext
    {
        public IdentityServerDbContext(DbContextOptions<IdentityServerDbContext> options) : base(options)
        {
        }
    }
}