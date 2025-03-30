namespace Vertr.Terminal.Components.Infrastructure;

public interface IStaticAssetService
{
    public Task<string?> GetAsync(string assetUrl, bool useCache = true);
}
