using PujcovadloServer.Business.Enums;

namespace PujcovadloServer.Responses;

public class ItemResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;

    public string Alias { get; set; } = default!;

    public ItemStatus Status { get; set; } = ItemStatus.Public;

    public float PricePerDay { get; set; }

    public float? RefundableDeposit { get; set; }

    public float? SellingPrice { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public double? Distance { get; set; }

    public UserResponse Owner { get; set; } = default!;

    public ImageResponse? MainImage { get; set; }

    public IList<ImageResponse> Images { get; set; } = new List<ImageResponse>();

    public IList<LinkResponse> _links { get; set; } = new List<LinkResponse>();
}