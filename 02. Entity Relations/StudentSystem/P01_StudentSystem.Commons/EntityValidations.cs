namespace P01_StudentSystem.Commons;

public static class EntityValidations
{
    public static class Student
    {
        public const int NameMaxLength = 100;

        public const bool PhoneNumberUnicode = false;
        
        public const int PhoneNumberMaxLength = 10;
    }

    public static class Course
    {
        public const int NameMaxLength = 80;
        
        public const int DescriptionMaxLength = 2000;

        public const string AmountColumnType = "DECIMAL(10,4)";
    }
    
    public static class Resource
    {
        public const int NameMaxLength = 50;
        
        public const bool UrlUnicode = false;
        
        public const int UrlMaxLength = 2048;
    }

    public static class Homework
    {
        public const bool ContentUnicode = false;

        public const int ContentMaxLength = 255;
    }
}