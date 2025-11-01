namespace BookShop.Data.EntityConfiguration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Models;
    using static Common.EntityValidation.Book;

    internal class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.HasKey(e => e.BookId);

            builder.Property(e => e.Title)
                .IsUnicode()
                .IsRequired()
                .HasMaxLength(TitleMaxLength);

            builder.Property(e => e.Description)
                .IsUnicode()
                .IsRequired()
                .HasMaxLength(DescriptionMaxLength);

            builder.Property(e => e.ReleaseDate)
                .IsRequired(IsReleaseDateRequired);

            builder.Property(e => e.Price)
                .HasColumnType(PriceFormat);

            builder.HasOne(e => e.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(e => e.AuthorId);
        }
    }
}
