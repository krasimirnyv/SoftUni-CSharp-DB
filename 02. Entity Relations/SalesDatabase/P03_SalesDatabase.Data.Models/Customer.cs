using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace P03_SalesDatabase.Data.Models;

using static Commons.EntityValidations.Customer;

public class Customer
{
    [Key]
    public int CustomerId { get; set; }

    [Required]
    [Unicode]
    [MaxLength(NameMaxLength)]
    public string Name { get; set; } = null!;

    [Required]
    [Unicode(EmailUnicode)]
    [MaxLength(EmailMaxLength)]
    public string Email { get; set; } = null!;
    
    [Required]
    [MaxLength(CreditCardNumberMaxLength)]
    public string CreditCardNumber { get; set; } = null!;

    public virtual ICollection<Sale> Sales { get; set; }
        = new HashSet<Sale>();
}