﻿using System.Reflection;

namespace BookShop.Data
{
    using Microsoft.EntityFrameworkCore;

    using Models;
    using EntityConfiguration;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    public class BookShopContext : DbContext
    {
        public BookShopContext()
        {
            
        }

        public BookShopContext(DbContextOptions options)
            : base(options)
        {
            
        }

        public virtual DbSet<Book> Books { get; set; } = null!;

        public virtual DbSet<Category> Categories { get; set; } = null!;

        public virtual DbSet<Author> Authors { get; set; } = null!;

        public virtual DbSet<BookCategory> BooksCategories { get; set; } = null!;
        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }
    }
}
