using PujcovadloServer.Enums;
using PujcovadloServer.Models;
using PujcovadloServer.Repositories.Interfaces;

namespace PujcovadloServer.Services;

public class ItemService(IItemRepository repository) : ACrudService<Item>(repository)
{
    
    public override async Task Update(Item item)
    {
        item.UpdatedAt = DateTime.Now;
        await base.Update(item);
    }

    public override Task Create(Item entity)
    {
        entity.CreatedAt = DateTime.Now;
        return base.Create(entity);
    }
    
    public override Task Delete(Item entity)
    {
        entity.DeletedAt = DateTime.Now;
        entity.Status = ItemStatus.Deleted;
        
        return base.Delete(entity);
    }
}