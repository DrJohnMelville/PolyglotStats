using FluentAssertions;
using Melville.PolyglotStats.TableSource.Parser;

namespace Melville.PolyglotStats.Test.TableSource.Parser;

public class MarkdownTableDetectorTest
{
    [Theory]
    [InlineData("""
      A | b | C
      ---|---|---|
      1|2|3
      """, true)]
    [InlineData("""
      A | b | C
      |---|---|---|
      1|2|3
      """, true)]
    [InlineData("""
      A | b | C
      |---|---|---
      1|2|3
      """, true)]
    [InlineData("""
      |---|---|---
      1|2|3
      """, false)]
    [InlineData("""
      A | b | C
      1|2|3
      """, false)]
    [InlineData("""
      A | b | C
      |---|-b--|---
      1|2|3
      """, false)]
    public void DetectMarkdown(string item, bool isMarkdowm) => 
        TableSourceClassifier.IsMarkdown(item).Should().Be(isMarkdowm);

    [Theory]
    [InlineData("Hello, world", true)]
    [InlineData("H,ello, worl,d", true)]
    [InlineData(",H,ello, worl,d,", true)]
    [InlineData("NotCsv", false)]
    public void DetectCsv(string item, bool isCsv) => 
        TableSourceClassifier.IsCsv(item).Should().Be(isCsv);


}