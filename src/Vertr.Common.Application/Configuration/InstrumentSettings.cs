namespace Vertr.Common.Application.Configuration;

public class InstrumentSettings
{
    public Guid BasicAsset { get; set; }

    public Guid[] DerivedAssets { get; set; } = [];
}
