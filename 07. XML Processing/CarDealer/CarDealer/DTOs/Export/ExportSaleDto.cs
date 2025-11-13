using System.Xml.Serialization;

namespace CarDealer.DTOs.Export;

[XmlType("sale")]
public class ExportSaleDto
{
    [XmlElement("car")]
    public ExportCarForSaleDto Car { get; set; } = null!;

    [XmlElement("discount")]
    public string Discount { get; set; } = null!;

    [XmlElement("customer-name")]
    public string CustomerName { get; set; } = null!;

    [XmlElement("price")]
    public string Price { get; set; } = null!;

    [XmlElement("price-with-discount")]
    public string PriceWithDiscount { get; set; } = null!;
}