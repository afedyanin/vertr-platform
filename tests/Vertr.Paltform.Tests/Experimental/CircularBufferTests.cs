using Microsoft.VisualStudio.Utilities;

namespace Vertr.Platform.Tests.Experimental;
public class CircularBufferTests
{
    [Test]
    public void CanCheckCb()
    {
        var cb = new CircularBuffer<int>(10);

        for (var i = 0; i < 10; i++)
        {
            cb.Add(i);
        }

        foreach (var i in cb)
        {
            Console.WriteLine(i);
        }

        for (var i = 10; i < 100; i++)
        {
            cb.Add(i);
        }

        foreach (var i in cb)
        {
            Console.WriteLine(i);
        }
    }
}
