namespace Vertr.Common.ForecastClient;

public static class Predictors
{
    public static class Stats
    {
        public const string Naive = "naive";
        public const string AutoArima = "auto_arima";
        public const string AutoEts = "auto_ets";
        public const string AutoCes = "auto_ces";
        public const string AutoTheta = "auto_theta";
        public const string RandomWalk = "random_walk";
        public const string HistoryAverage = "history_average";

        public static string[] AllKeys => [Naive, AutoArima, AutoEts, AutoCes, AutoTheta, RandomWalk, HistoryAverage];
    }

    public static class Ml
    {
        public const string Lgbm = "lgbm";
        public const string Lasso = "lasso";
        public const string LinReg = "lin_reg";
        public const string Ridge = "ridge";
        public const string Knn = "knn";
        public const string Mlp = "mlp";
        public const string Rf = "rf";
        public static string[] AllKeys => [Lgbm, Lasso, LinReg, Ridge, Knn, Mlp, Rf];
    }

    public static class Neural
    {
        public const string AutoLSTM = "AutoLSTM";
        public const string AutoRNN = "AutoRNN";
        public const string AutoMLP = "AutoMLP";
        public const string AutoDeepAR = "AutoDeepAR";
        public const string AutoDeepNPTS = "AutoDeepNPTS";
        public const string AutoKAN = "AutoKAN";
        public const string AutoTFT = "AutoTFT";
        public const string AutoTimesNet = "AutoTimesNet";

        public static string[] AllKeys => [AutoLSTM, AutoRNN, AutoMLP, AutoDeepAR, AutoDeepNPTS, AutoKAN, AutoTFT, AutoTimesNet];
    }
}
