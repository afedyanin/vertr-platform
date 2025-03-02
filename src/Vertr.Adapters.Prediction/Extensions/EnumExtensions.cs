using Vertr.Domain.Enums;

namespace Vertr.Adapters.Prediction.Extensions;

internal static class EnumExtensions
{
    public static string GetName(this PredictorType predictor)
        => predictor switch
        {
            PredictorType.Undefined => "Undefined",
            PredictorType.RandomWalk => "RandomWalk",
            PredictorType.Sb3 => "Sb3",
            PredictorType.TrendFollowing => "TrendFollowing",
            _ => throw new InvalidOperationException($"Unknown PredictorType={predictor}"),
        };

    public static string GetName(this Sb3Algo algo)
        => algo switch
        {
            Sb3Algo.Undefined => "Undefined",
            Sb3Algo.A2C => "a2c",
            Sb3Algo.DDPG => "ddpg",
            Sb3Algo.DQN => "dqn",
            Sb3Algo.PPO => "ppo",
            Sb3Algo.SAC => "sac",
            Sb3Algo.TD3 => "td3",
            Sb3Algo.ARS => "ars",
            Sb3Algo.QRDQN => "qrdqn",
            Sb3Algo.TQC => "tqc",
            Sb3Algo.TRPO => "trpo",
            Sb3Algo.RecurrentPPO => "ppo_lstm",
            _ => throw new InvalidOperationException($"Unknown Sb3Algo={algo}"),
        };
}
