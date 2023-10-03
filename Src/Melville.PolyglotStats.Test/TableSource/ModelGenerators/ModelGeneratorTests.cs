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
                                          public record TableClass (
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
                                          public record TableClass (
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
                                          public readonly TableClass[] Table = new TableClass[] {
                                              new (1, 2),
                                              new (3, 4),
                                          };
                                      
                                      """);
    }

}