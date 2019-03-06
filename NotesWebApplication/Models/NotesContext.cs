using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NotesWebApplication.Models
{
    public class NotesContext : DbContext
    {
        public DbSet<Note> Notes { get; set; }
        public NotesContext(DbContextOptions<NotesContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
