namespace BookShop.Common;

public static class EntityValidation
{
    public static class Author
    {
        public const int FirstNameMaxLength = 50;

        public const bool IsFirstNameRequired = false;
        
        public const int LastNameMaxLength = 50;
    }

    public static class Book
    {
        public const int TitleMaxLength = 50;
        
        public const int DescriptionMaxLength = 1000;
        
        public const bool IsReleaseDateRequired = false;

        public const string PriceFormat = "DECIMAL(12, 5)";
    }

    public static class Category
    {
        public const int NameMaxLength = 50;
    }
}