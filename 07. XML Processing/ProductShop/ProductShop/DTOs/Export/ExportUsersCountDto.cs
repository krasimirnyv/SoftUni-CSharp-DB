using System.Xml.Serialization;

namespace ProductShop.DTOs.Export;

[XmlRoot("Users")]
public class ExportUsersCountDto
{
    [XmlElement("count")]
    public int TotalUsersCount { get; set; }

    [XmlArray("users")]
    public ExportUserWithSoldProductsDto[] Users { get; set; } = null!;
}