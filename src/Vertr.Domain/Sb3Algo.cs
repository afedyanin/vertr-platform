namespace Vertr.Domain;
public record class Sb3Algo(string Name)
{
    public static readonly Sb3Algo A2C = new Sb3Algo("a2c");
    public static readonly Sb3Algo DDPG = new Sb3Algo("ddpg");
    public static readonly Sb3Algo DQN = new Sb3Algo("dqn");
    public static readonly Sb3Algo PPO = new Sb3Algo("ppo");
    public static readonly Sb3Algo SAC = new Sb3Algo("sac");
    public static readonly Sb3Algo TD3 = new Sb3Algo("td3");
    public static readonly Sb3Algo ARS = new Sb3Algo("ars");
    public static readonly Sb3Algo QRDQN = new Sb3Algo("qrdqn");
    public static readonly Sb3Algo TQC = new Sb3Algo("tqc");
    public static readonly Sb3Algo TRPO = new Sb3Algo("trpo");
    public static readonly Sb3Algo RecurrentPPO = new Sb3Algo("ppo_lstm");
}
