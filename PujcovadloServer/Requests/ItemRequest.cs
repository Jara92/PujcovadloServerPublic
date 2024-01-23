using System.ComponentModel.DataAnnotations;
using PujcovadloServer.Enums;
using PujcovadloServer.Models;

namespace PujcovadloServer.Requests;

public class ItemRequest
{
    public int Id { get; set; }
    
    public string Name { get; set; } = default!;
    
    public string Description { get; set; } = default!;
    
    public string Parameters { get; set; } = "";
    
    public float PricePerDay { get; set; }
    
    public float? RefundableDeposit { get; set; }
    
    public float? SellingPrice { get; set; }
    
    public float? PurchasePrice { get; set; }

    // todo make DTOs
    public virtual ICollection<ItemCategoryRequest> Categories { get; } = new List<ItemCategoryRequest>();
}