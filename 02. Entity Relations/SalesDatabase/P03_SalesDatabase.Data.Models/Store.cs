using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace P03_SalesDatabase.Data.Models;

using static Commons.EntityValidations.Store;

public class Store
{
    [Key]
    public int StoreId { get; set; }
    
    [Required]
    [Unicode]
    [MaxLength(NameMaxLength)]
    public string Name { get; set; } = null!;

    public virtual ICollection<Sale> Sales { get; set; }
        = new HashSet<Sale>();
}