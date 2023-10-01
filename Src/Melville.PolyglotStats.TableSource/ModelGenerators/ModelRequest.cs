using System;
using System.Collections;
using System.Collections.Generic;
using Melville.INPC;
using Melville.PolyglotStats.TableSource.TypeInference;

namespace Melville.PolyglotStats.TableSource.ModelGenerators;

public readonly partial struct FieldRequest
{
    [FromConstructor] public Memory<Char> Name { get; }
    [FromConstructor] public InferredType Type { get; }
}

public readonly partial struct ModelRequest
{
    [FromConstructor] public readonly IList<FieldRequest> Fields { get; }
}