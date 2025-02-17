namespace Vertr.Domain;

public record class PredictorType(string Name)
{
    public static readonly PredictorType Undefined = new PredictorType("Undefined");
    public static readonly PredictorType Sb3 = new PredictorType("Sb3");
    public static readonly PredictorType RandomWalk = new PredictorType("RandomWalk");
    public static readonly PredictorType TrendFollowing = new PredictorType("TrendFollowing");
}
