using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using FSharp.Compiler.Syntax;
using Melville.PolyglotStats.TableSource.ModelGenerators;
using Melville.PolyglotStats.TableSource.TypeInference;

namespace Melville.PolyglotStats.Test.TableSource.ModelGenerators;

public class ModelGeneratorTests
{
    private readonly StringBuilder target = new();

    private void SingleTypeTest(InferredType type, string typeName)
    {
        var request = new ModelBuilder("Table".AsMemory(),
            new FieldRequest("Col1".AsMemory(), type));
        
        request.WriteTypeDeclarationTo(target);

        target.ToString().Should().Be($"""
                                          public partial record Table (
                                              {typeName} Col1
                                          );
                                      
                                      """);
    }

    [Fact] public void OneIntModel() => SingleTypeTest(InferredNumberType<int>.Instance, "System.Int32");
    [Fact] public void OneUIntModel() => SingleTypeTest(InferredNumberType<uint>.Instance, "System.UInt32");
    [Fact] public void OneNullableIntModel() => SingleTypeTest(InferredNumberType<int>.Instance.SelectByNullability(true), "System.Int32?");
    [Fact] public void OneBoolModel() => SingleTypeTest(InferredBooleanType.Instance, "bool");
    [Fact] public void OneStringModel() => SingleTypeTest(InferredStringType.Instance, "string");


    [Fact]
    public void TwoIntModel()
    {
        var request = new ModelBuilder("Table".AsMemory(),
            new FieldRequest("Col1".AsMemory(), InferredNumberType<int>.Instance),
            new FieldRequest("Col2".AsMemory(), InferredNumberType<int>.Instance));
        
        request.WriteTypeDeclarationTo(target);

        target.ToString().Should().Be("""
                                          public partial record Table (
                                              System.Int32 Col1,
                                              System.Int32 Col2
                                          );
                                      
                                      """);
    }

    [Fact]
    public void GenerateDataDeclarations()
    {
        var request = new ModelBuilder("Table".AsMemory(),
            new FieldRequest("Col1".AsMemory(), InferredNumberType<int>.Instance),
            new FieldRequest("Col2".AsMemory(), InferredNumberType<int>.Instance));
        var data = new[]
        {
            new[] { "1".AsMemory(), "2".AsMemory() },
            new[] { "3".AsMemory(), "4".AsMemory() },
        };

        request.WriteDataTo(target, data);
        target.ToString().Should().Be("""
                                          public readonly Table[] Table = new Table[] {
                                              new (1, 2),
                                              new (3, 4),
                                          };
                                      
                                      """);
    }

    [Theory]
    [InlineData("2", "2")]
    [InlineData("1e6", "1e6")]
    [InlineData("1.5", "1.5")]
    [InlineData("Hello", "\"Hello\"")]
    [InlineData("t", "true")]
    [InlineData("", "default")]
    [InlineData("1/2/33", "System.DateTime.Parse(\"1/2/33\")")]
    public void PrintValue(string source, string destination)
    {
        var type = InferType.Of(new[] { source.AsMemory() }).SelectByNullability(true);
        type.WriteValue(source.AsMemory(), target);
        target.ToString().Should().Be(destination);
    }
}