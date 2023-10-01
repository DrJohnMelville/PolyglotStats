using System;
using System.Text;
using FSharp.Compiler.SyntaxTrivia;

namespace Melville.PolyglotStats.TableSource.TypeInference;

public abstract class InferredType
{
    protected InferredType AsNullable { get; set; }
    public InferredType SelectByNullability(bool nullable) => nullable ? AsNullable : this;

    protected InferredType()
    {
        AsNullable = this is NullableWrapper or InferredStringType? this: new NullableWrapper(this);
    }

    public abstract bool CanParse(ReadOnlyMemory<char> datum);
    public abstract void WriteTypeName(StringBuilder target);
}