namespace Vertr.TinvestGateway.Tests.Stubs;

public class NormalRandom
{
    private readonly Random _random;
    private readonly double _mean;
    private readonly double _stdDev;
    private bool _hasSpare; // To store the second number from Box-Muller
    private double _spareValue;

    public NormalRandom(double mean = 0.0, double stdDev = 1.0)
    {
        _random = new Random();
        _mean = mean;
        _stdDev = stdDev;
        _hasSpare = false;
    }

    public double NextDouble()
    {
        if (_hasSpare)
        {
            _hasSpare = false;
            return _spareValue * _stdDev + _mean;
        }
        else
        {
            var u1 = _random.NextDouble();
            var u2 = _random.NextDouble();

            var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            _spareValue = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
            _hasSpare = true;

            return randStdNormal * _stdDev + _mean;
        }
    }
}