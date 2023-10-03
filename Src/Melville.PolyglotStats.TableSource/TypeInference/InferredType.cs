using System;
using System.Text;
using FSharp.Compiler.SyntaxTrivia;
using Melville.PolyglotStats.TableSource.MemorySerializer;

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

    public abstract void WriteValue(MemoryWriter writer, ReadOnlyMemory<char> value);

    public virtual void WriteReader(StringBuilder target)
    {
        target.Append("reader.Read<");
        WriteTypeName(target);
        target.Append(">()");
    }
}