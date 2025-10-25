namespace P03_SalesDatabase.Commons;

public static class EntityValidations
{
    public static class Product
    {
        public const int NameMaxLength = 50;
        
        public const string AmountQuantity = "DECIMAL(10, 4)";
        
        public const string AmountPrice = "DECIMAL(18, 4)";
    }

    public static class Customer
    {
        public const int NameMaxLength = 100;
        
        public const int EmailMaxLength = 80;
        
        public const bool EmailUnicode = false;
        
        public const int CreditCardNumberMaxLength = 20;
        
        public const string AmountSales = "DECIMAL(18, 4)";
    }
    
    public static class Store
    {
        public const int NameMaxLength = 80;
        
        public const string AmountSales = "DECIMAL(18, 4)";
    }
}