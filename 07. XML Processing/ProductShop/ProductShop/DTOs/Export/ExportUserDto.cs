using System.Xml.Serialization;

namespace ProductShop.DTOs.Export;

[XmlType("User")]
public class ExportUserDto
{
    [XmlElement("firstName")]
    public string? FirstName { get; set; }

    [XmlElement("lastName")]
    public string LastName { get; set; } = null!;

    [XmlArray("soldProducts")]
    public ExportSoldProductDto[] SoldProducts { get; set; } = null!;
}