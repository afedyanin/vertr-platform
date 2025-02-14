namespace Vertr.Adapters.Prediction.Models;
internal record class Algo(string Name)
{
    public static readonly Algo A2C = new Algo("a2c");
    public static readonly Algo DDPG = new Algo("ddpg");
    public static readonly Algo DQN = new Algo("dqn");
    public static readonly Algo PPO = new Algo("ppo");
    public static readonly Algo SAC = new Algo("sac");
    public static readonly Algo TD3 = new Algo("td3");
    public static readonly Algo ARS = new Algo("ars");
    public static readonly Algo QRDQN = new Algo("qrdqn");
    public static readonly Algo TQC = new Algo("tqc");
    public static readonly Algo TRPO = new Algo("trpo");
    public static readonly Algo RecurrentPPO = new Algo("ppo_lstm");
}

/*
ALGOS: Dict[str, Type[BaseAlgorithm]] = {
    "a2c": A2C,
    "ddpg": DDPG,
    "dqn": DQN,
    "ppo": PPO,
    "sac": SAC,
    "td3": TD3,
    # SB3 Contrib,
    "ars": ARS,
    "qrdqn": QRDQN,
    "tqc": TQC,
    "trpo": TRPO,
    "ppo_lstm": RecurrentPPO,
}
*/
