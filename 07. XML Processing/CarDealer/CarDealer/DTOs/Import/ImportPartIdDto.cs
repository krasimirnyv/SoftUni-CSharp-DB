using System.Xml.Serialization;

namespace CarDealer.DTOs.Import;

[XmlType("partId")]
public class ImportPartIdDto
{
    [XmlAttribute("id")]
    public string Id { get; set; } = null!;
}