using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using FSharp.Compiler.AbstractIL;
using FSharp.Compiler.Syntax;
using Melville.PolyglotStats.TableSource.ModelGenerators;
using Melville.PolyglotStats.TableSource.TypeInference;

namespace Melville.PolyglotStats.Test.TableSource.ModelGenerators;

public class ModelGeneratorTests
{
    private readonly StringBuilder target = new();
    private readonly StringBuilder documentation  = new();

    private void SingleTypeTest(InferredType type, string typeName)
    {
        var request = new ModelBuilder("Table".AsMemory(),
            new []{new FieldRequest("Col1".AsMemory(), type)},
                    target, documentation);
        
        request.WriteTypeDeclarationTo();

        target.ToString().Should().Be($"""
                                          public record TableClass (
                                              {typeName} Col1
                                          );
                                      
                                      """);

        documentation.ToString().Should().Be($"""
                                  <div><details>
                                  <summary>Table</summary>
                                  <table>
                                  <tr><td>{typeName}</td><td>Col1</td><tr>
                                  </table>
                                  </details></div>
                                  
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
            new []{
            new FieldRequest("Col1".AsMemory(), InferredNumberType<int>.Instance),
            new FieldRequest("Col2".AsMemory(), InferredNumberType<int>.Instance)},
            target, documentation);
        
        request.WriteTypeDeclarationTo();

        target.ToString().Should().Be("""
                                          public record TableClass (
                                              System.Int32 Col1,
                                              System.Int32 Col2
                                          );
                                      
                                      """);
    }
}