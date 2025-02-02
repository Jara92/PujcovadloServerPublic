using PujcovadloServer.Business.Entities;
using PujcovadloServer.Business.Filters;
using PujcovadloServer.Business.Objects;

namespace PujcovadloServer.Business.Interfaces;

public interface IItemCategoryRepository : ICrudRepository<ItemCategory, ItemCategoryFilter>
{
    public Task<IList<ItemCategory>> GetByIds(IEnumerable<int> ids);

    Task<List<EntityOption>> GetAllOptions();
}