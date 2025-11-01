namespace BookShop.Data.EntityConfiguration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Models;
    using static Common.EntityValidation.Category;

    internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(e => e.CategoryId);

            builder.Property(e => e.Name)
                .IsUnicode()
                .IsRequired()
                .HasMaxLength(NameMaxLength);
        }
    }
}
