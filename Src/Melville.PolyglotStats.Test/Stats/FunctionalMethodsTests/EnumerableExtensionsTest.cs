using Melville.PolyglotStats.Stats.Functional;
using EnumerableExtensions = Melville.Linq.EnumerableExtensions;

namespace Melville.PolyglotStats.Test.Stats.FunctionalMethodsTests;

public class EnumerableExtensionsTest
{
    private void TestInterleave(string result, params string[] args)
    {
        Assert.Equal(result, EnumerableExtensions.ConcatenateStrings(EnumerableExtensions.Interleave(args, ",", "|")));
        Assert.Equal(result.Replace('|', ','), EnumerableExtensions.ConcatenateStrings(EnumerableExtensions.Interleave(args, ",")));
    }
    [Fact]
    public void InterleveTests()
    {
        TestInterleave("");
        TestInterleave("A", "A");
        TestInterleave("A|B", "AB".ToCharArray().Select(i => i.ToString()).ToArray());
        TestInterleave("A,B|C", "ABC".ToCharArray().Select(i => i.ToString()).ToArray());
        TestInterleave("A,B,C|D", "ABCD".ToCharArray().Select(i => i.ToString()).ToArray());
    }

    [Theory]
    [InlineData("", new object[0])]
    [InlineData("aa", new[] { "aa" })]
    [InlineData("aabb", new[] { "aa", "bb" })]
    [InlineData("aabbcccc", new[] { "aa", "bb", "cccc" })]

    public void ConcatStrings(string result, object[] items)
    {
        Assert.Equal(result, EnumerableExtensions.ConcatenateStrings(items));
    }

    [Fact]
    public void FirstOrDefaultTest()
    {
        Assert.Equal(10, new int[] { }.FirstOrDefault(10));
        Assert.Equal(1, new int[] { 1, 2, 3 }.FirstOrDefault(10));
    }

    [Theory]
    [InlineData(10, 1)]
    [InlineData(10, 3)]
    [InlineData(10, 9)]
    [InlineData(10, 10)]
    [InlineData(10, 11)]
    [InlineData(10, 25)]
    [InlineData(0, 25)]
    public void TestDecolate(int max, int cols)
    {
        var lists = EnumerableExtensions.Decolate<int>(Enumerable.Range(0, max), cols);
        for (int i = 0; i < lists.Length; i++)
        {
            Assert.True(lists[i].All(j => i == (j % cols)));
        }
    }

    [Fact]
    public void ConcatItemsTest()
    {
        Assert.Equal("1234", EnumerableExtensions.ConcatenateStrings(EnumerableExtensions.Concat((new int[] { 1, 2 }), 3, 4)));
    }
    [Fact]
    public void PrependTest()
    {
        Assert.Equal("3412", EnumerableExtensions.ConcatenateStrings(EnumerableExtensions.Prepend((new int[] { 1, 2 }), 3, 4)));
    }
    [Fact]
    public void PrependTest2()
    {
        Assert.Equal("3412", EnumerableExtensions.ConcatenateStrings(EnumerableExtensions.Prepend((new int[] { 1, 2 }), new int[] { 3, 4 }.AsEnumerable())));
    }
    private void InnerAllBeforeTest(int sentinal, int length, params int[] data)
    {
        var result = EnumerableExtensions.AllBefore(data, sentinal).ToArray();
        Assert.Equal(length, result.Length);
        int i = 0;
        Assert.True(result.All(elt => data[i++] == elt));
    }
    [Fact]
    public void AllBeforeTest()
    {
        InnerAllBeforeTest(3, 2, 1, 2, 3, 4, 5, 6);
        InnerAllBeforeTest(1, 0, 1, 2, 3, 4, 5, 6);
        InnerAllBeforeTest(109, 0);
        InnerAllBeforeTest(100, 6, 1, 2, 3, 4, 5, 6);
    }

    [Fact]
    public void TestEnglishList()
    {
        Assert.Equal("", EnumerableExtensions.EnglishList(new string[0]));
        Assert.Equal("a", EnumerableExtensions.EnglishList(new[] { "a" }));
        Assert.Equal("a and b", EnumerableExtensions.EnglishList(new[] { "a", "b" }));
        Assert.Equal("a, b, and c", EnumerableExtensions.EnglishList(new[] { "a", "b", "c" }));
        Assert.Equal("a, b, c, and d", EnumerableExtensions.EnglishList(new[] { "a", "b", "c", "d" }));

    }

    [Fact]
    public void TestForeachExtension()
    {
        int count = 0;
        EnumerableExtensions.ForEach(Enumerable.Range(0, 12), i => count++);
        Assert.Equal(12, count);

    }

    [Theory]
    [InlineData(0,"0123456789")]
    [InlineData(100,"0123456789")]
    [InlineData(1,"1234567890")]
    [InlineData(5, "5678901234")]
    public void TestRotateDirect(int rotation, string result)=>
        Assert.Equal(result, EnumerableExtensions.ConcatenateStrings(EnumerableExtensions.Rotate(Enumerable.Range(0,10), rotation)));

    

    [Theory]
    [InlineData(0, 10)]
    [InlineData(0, 0)]
    [InlineData(3, 10)]
    public void TestRotate(int rotation, int elementLength)
    {
        // setup the test
        Assert.True(rotation >= 0);
        Assert.True(rotation <= elementLength);
        var elements = Enumerable.Range(0, elementLength).ToArray();

        var newList = EnumerableExtensions.Rotate(elements, rotation).ToArray();

        Assert.Equal(elements.Length, newList.Length);
        for (int i = 0; i < elements.Length; i++)
        {
            Assert.Equal(elements[(i + rotation) % elements.Length], newList[i]);
        }
    }

    [Theory]
    [InlineData(new[] { 1 })]
    [InlineData(new[] { 1, 1, 1 })]
    [InlineData(new[] { 1, 2, 3 })]
    [InlineData(new[] { 3, 2, 1 })]
    [InlineData(new[] { 3, 2, 1, 2, 2, 2, 1 })]
    public void TestMaxMin(int[] data)
    {
        var result = EnumerableExtensions.MinMax(data);
        Assert.True(data.All(i => i <= result.Item2 && i >= result.Item1));
    }

  
    [Fact]
    public void SkipOver()
    {
        TestIntSequence(EnumerableExtensions.SkipOver(new[] { 1, 2, 3, 4, 5 }, 3), 4, 5);
        TestIntSequence(EnumerableExtensions.SkipOver(new[] { 1, 2, 3, 4, 5 }, 1), 2, 3, 4, 5);
        TestIntSequence(EnumerableExtensions.SkipOver(new[] { 1, 2, 3, 4, 5 }, 0), 1, 2, 3, 4, 5);
        TestIntSequence(EnumerableExtensions.SkipOver(new[] { 1, 2, 3, 4, 5 }, 5));
        TestIntSequence(EnumerableExtensions.SkipOver(new[] { 1, 2, 3, 4, 5 }, 6));
    }

    private void TestIntSequence(IEnumerable<int> a, params int[] b)
    {
        Assert.Equal(a, b);
    }

    [Fact]
    public void IndexOfTest()
    {
        var seq = Enumerable.Range(0, 15).ToArray();
        for (int i = 0; i < seq.Length; i++)
        {
            Assert.Equal(i, EnumerableExtensions.IndexOf(seq, i));
        }
    }

    [Fact]
    public void IndexOfNotFoundIsMinus1()
    {
        Assert.Equal(-1, EnumerableExtensions.IndexOf(new int[0], 5));
    }

    [Fact]
    public void SideEffectTest()
    {
        var sum = 0;
        Assert.Equal(55, EnumerableExtensions.SideEffect(Enumerable.Range(1, 10), i => sum += i).Sum());
        Assert.Equal(55, sum);

    }

    [Fact]
    public void Cycle()
    {
        IEnumerable<int> tempQualifier = Enumerable.Range(1,3).Cycle(9);
        Assert.Equal("123123123", string.Join(string.Empty, tempQualifier.ToArray()));
    }

}