using System.Text;
using System.Text.RegularExpressions;
using FSharp.Compiler.Syntax;
using Melville.PolyglotStats.TableSource.ModelGenerators;
using Melville.PolyglotStats.TableSource.TypeInference;

namespace Melville.PolyglotStats.Test.TableSource.ModelGenerators;

public class ModelGeneratorTests
{
    public void SingleTypeTest(InferredType type, string typeName)
    {
        var request = new ModelBuilder("Table".AsMemory(),
            new FieldRequest("Col1".AsMemory(), type));
        var target = new StringBuilder();
        
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
        var target = new StringBuilder();
        
        request.WriteTypeDeclarationTo(target);

        target.ToString().Should().Be("""
                                          public partial record Table (
                                              System.Int32 Col1,
                                              System.Int32 Col2
                                          );
                                      
                                      """);
    }
}