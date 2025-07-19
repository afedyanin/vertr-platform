namespace Vertr.Platform.Tests.Experimental;

public class SortedCollectionTests
{
    [Test]
    public void SortedDictTest()
    {
        var sortedDict = new SortedDictionary<string, int>();
        sortedDict["banana"] = 3;
        sortedDict["apple"] = 4;
        sortedDict["pear"] = 1;
        sortedDict["orange"] = 2;

        Console.WriteLine($"first={sortedDict.First()}");
        Console.WriteLine($"last={sortedDict.Last()}");

        var top2 = sortedDict.Take(2);
        var bottom2 = sortedDict.TakeLast(2);

        var top2str = string.Join(", ", top2);
        var bottom2str = string.Join(", ", bottom2);

        Console.WriteLine($"top 2 ={top2str}");
        Console.WriteLine($"bottom 2 = {bottom2str}");

        var top10 = sortedDict.Take(10);
        var top10str = string.Join(", ", top10);
        Console.WriteLine($"top 10 ={top10str}");
    }

    [Test]
    public void SortedListTest()
    {
        var sortedList = new SortedList<string, int>();
        sortedList["banana"] = 3;
        sortedList["apple"] = 4;
        sortedList["pear"] = 1;
        sortedList["orange"] = 2;

        Console.WriteLine($"first={sortedList.First()}");
        Console.WriteLine($"last={sortedList.Last()}");

        var top2 = sortedList.Take(2);
        var bottom2 = sortedList.TakeLast(2);

        var top2str = string.Join(", ", top2);
        var bottom2str = string.Join(", ", bottom2);

        Console.WriteLine($"top 2 ={top2str}");
        Console.WriteLine($"bottom 2 = {bottom2str}");

        var top10 = sortedList.Take(10);
        var top10str = string.Join(", ", top10);
        Console.WriteLine($"top 10 ={top10str}");

        sortedList.Remove(sortedList.First().Key);
        sortedList["melon"] = 24;

        top10 = sortedList.Take(10);
        top10str = string.Join(", ", top10);
        Console.WriteLine($"top 10 ={top10str}");
    }

    [Test]
    public void CanAddSameValueTwice()
    {
        var sortedList = new SortedList<string, int>();
        sortedList["banana"] = 3;
        sortedList["apple"] = 4;
        sortedList["pear"] = 1;

        sortedList["orange"] = 2;
        sortedList["orange"] = 8;
        // sortedList.Add("orange", 8);

        var allStr = string.Join(", ", sortedList);
        Console.WriteLine($"{allStr}");
    }
}
