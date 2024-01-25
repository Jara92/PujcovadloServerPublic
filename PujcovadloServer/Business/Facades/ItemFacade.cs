using AutoMapper;
using PujcovadloServer.Authentication;
using PujcovadloServer.Business.Entities;
using PujcovadloServer.Business.Enums;
using PujcovadloServer.Business.Exceptions;
using PujcovadloServer.Business.Filters;
using PujcovadloServer.Business.Interfaces;
using PujcovadloServer.Business.Services;
using PujcovadloServer.Business.Services.Interfaces;
using PujcovadloServer.Helpers;
using PujcovadloServer.Lib;
using PujcovadloServer.Requests;

namespace PujcovadloServer.Business.Facades;

public class ItemFacade
{
    private readonly IItemRepository _itemRepository;
    private readonly ItemService _itemService;
    private readonly IAuthenticateService _authenticateService;
    private readonly IMapper _mapper;

    public ItemFacade(IItemRepository itemRepository, ItemService itemService, IAuthenticateService authenticateService, IMapper mapper)
    {
        _itemRepository = itemRepository;
        _itemService = itemService;
        _authenticateService = authenticateService;
        _mapper = mapper;
    }

    /// <summary>
    /// Creates a new item using <see cref="ItemRequest"/>
    /// </summary>
    /// <param name="request"></param>
    public async Task<Item> CreateItem(ItemRequest request)
    {
        var user = await _authenticateService.GetCurrentUser();
        if(user == null) throw new UnauthorizedAccessException("User not found.");
        
        // Map request to item
        var item = _mapper.Map<Item>(request);

        // Set initial status
        item.Status = ItemStatus.Public;

        // Set alias
        item.Alias = UrlHelper.CreateUrlStub(item.Name);
        
        // Set owner
        item.Owner = user;

        // Create the item  
        await _itemService.Create(item);

        return item;
    }

    public async Task UpdateItem(ItemRequest request)
    {
        // Updated tracked item
        var item = await _itemRepository.Get(request.Id);

        if (item == null) throw new EntityNotFoundException($"Item with id {request.Id} not found.");

        // Map request to item
        item.Name = request.Name;
        item.Alias = UrlHelper.CreateUrlStub(item.Name);
        item.Description = request.Description;

        // update prices
        item.PricePerDay = request.PricePerDay;
        item.PurchasePrice = request.PurchasePrice;
        item.SellingPrice = request.SellingPrice;

        // Update categories
        item.Categories.Clear();
        foreach (var category in request.Categories)
        {
            item.Categories.Add(_mapper.Map<ItemCategory>(category));
        }
        
        // Item updated so we need to approve it
        if(item.Status == ItemStatus.Denied)
            item.Status = ItemStatus.Approving;
        
        // Todo: update images
        
        // Update the item
        await _itemService.Update(item);
    }
    
    public async Task DeleteItem(int id)
    {
        // Get the item
        var item = await _itemService.Get(id);
        if (item == null) throw new EntityNotFoundException($"Item with id {id} not found.");
        
        // TODO: Check that the item can be deleted (no active rentals etc.)
        
        // Delete the item
        await _itemService.Delete(item);
    }
    
    public async Task<PaginatedList<Item>> GetMyItems(ItemFilter filter)
    {
        var user = await _authenticateService.GetCurrentUser();
        
        if (user == null) throw new UnauthorizedAccessException("User not found.");
        
        // Set owner id
        filter.OwnerId = user.Id;
        
        // Get items
        var items = await _itemService.GetAll(filter);
        
        return items;
    }
}