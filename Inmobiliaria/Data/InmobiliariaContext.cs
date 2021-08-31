using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Inmobiliaria.Models;

namespace Inmobiliaria.Data
{
    public class InmobiliariaContext : DbContext
    {
        public InmobiliariaContext (DbContextOptions<InmobiliariaContext> options)
            : base(options)
        {
        }

        public DbSet<Inmobiliaria.Models.Pago> Pago { get; set; }

        public DbSet<Inmobiliaria.Models.Usuario> Usuario { get; set; }
    }
}
