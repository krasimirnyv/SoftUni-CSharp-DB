using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace P03_SalesDatabase.Data.Models;

using static Commons.EntityValidations.Product;

public class Product
{
    [Key]
    public int ProductId { get; set; }

    [Unicode]
    [Required]
    [MaxLength(NameMaxLength)]
    public string Name { get; set; } = null!;

    [Column(TypeName = AmountQuantity)]
    public decimal Quantity { get; set; }
    
    [Column(TypeName = AmountPrice)]
    public decimal Price { get; set; }
    
    public virtual ICollection<Sale> Sales { get; set; } 
        = new HashSet<Sale>();
}
