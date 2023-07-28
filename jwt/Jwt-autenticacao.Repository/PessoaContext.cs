using Jwt_autenticacao.Repository.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jwt_autenticacao.Repository
{
    public class PessoaContext : DbContext
    {

         public PessoaContext(DbContextOptions<PessoaContext> options): base(options) { }

        public DbSet<TabUsuario>? Usuarios { get; set; }
        public DbSet<TabPessoa>? Pessoa { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<TabUsuario>().ToTable("tabUsuario");
            modelBuilder.Entity<TabPessoa>().ToTable("tabPessoa");
        }
    }
}
