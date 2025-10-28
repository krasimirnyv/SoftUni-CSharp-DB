namespace MusicHub.Common;

public static class EntityValidation
{
    public static class Song
    {
        public const int NameMaxLength = 20;

        public const string PriceFormat = "DECIMAL(10, 5)";
    }
    
    public static class Album
    {
        public const int NameMaxLength = 40;
    }
    
    public static class Performer
    {
        public const int FirstNameMaxLength = 20;

        public const int LastNameMaxLength = 20;

        public const string MoneyFormat = "DECIMAL(18, 5)";
    }
    
    public static class Producer
    {
        public const int NameMaxLength = 30;

        public const int PseudonymMaxLength = 200;

        public const int PhoneNumberMaxLength = 15;
    }

    public static class Writer
    {
        public const int NameMaxLength = 20;
        
        public const int PseudonymMaxLength = 200;
    }
}