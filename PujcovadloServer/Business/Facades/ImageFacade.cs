using PujcovadloServer.Business.Entities;
using PujcovadloServer.Business.Exceptions;
using PujcovadloServer.Business.Interfaces;
using PujcovadloServer.Business.Services;
using PujcovadloServer.Business.Services.Interfaces;

namespace PujcovadloServer.Business.Facades;

public class ImageFacade
{
    private readonly ImageService _imageService;
    private readonly IAuthenticateService _authenticateService;
    private readonly PujcovadloServerConfiguration _configuration;
    private readonly IFileStorage _fileStorage;

    public ImageFacade(ImageService imageService, IAuthenticateService authenticateService, IFileStorage fileStorage,
        PujcovadloServerConfiguration configuration)
    {
        _imageService = imageService;
        _authenticateService = authenticateService;
        _fileStorage = fileStorage;
        _configuration = configuration;
    }

    public virtual async Task<Image> CreateImage(Image image, string filePath)
    {
        var user = await _authenticateService.GetCurrentUser();

        // Current user is the owner of the image
        image.Owner = user;

        // Move the file to the images directory
        image.Path = await _fileStorage.Save(filePath, _configuration.ImagesPath, image.MimeType, image.Extension);

        // save the image
        await _imageService.Create(image);

        return image;
    }

    public async Task<string> GetImagePath(Image image)
    {
        return await _fileStorage.GetFilePublicUrl(_configuration.ImagesPath, image.Path);
    }

    public virtual async Task<Image> GetImage(int imageId)
    {
        // Get image and check that it is not null
        var image = await _imageService.Get(imageId);
        if (image == null) throw new EntityNotFoundException("Image not found");

        return image;
    }

    public virtual Task<Image?> GetImageOrNull(int imageId)
    {
        return _imageService.Get(imageId);
    }

    public async Task<Image> GetImage(string name)
    {
        var image = await _imageService.GetByPath(name);
        if (image == null) throw new EntityNotFoundException("Image not found.");

        return image;
    }

    public virtual async Task DeleteImage(Image image)
    {
        await _imageService.Delete(image);

        await _fileStorage.Delete(_configuration.ImagesPath, image.Path);
    }
}