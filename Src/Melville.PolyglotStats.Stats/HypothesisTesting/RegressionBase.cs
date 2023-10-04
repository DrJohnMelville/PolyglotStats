using Melville.PolyglotStats.Stats.Functional;
using Melville.PolyglotStats.Stats.Tables;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;
using System.Linq;

namespace Melville.PolyglotStats.Stats.HypothesisTesting;

public abstract class RegressionBase<T, TResult, TFinalType> where TResult : struct
{
    protected Func<T, TResult> resultsFunc { get; }
    private readonly IList<T> rawItems;


    protected RegressionBase(IEnumerable<T> items, Func<T, TResult> resultsFunc)
    {
        rawItems = items.AsList();
        Filter = new UnknownFilter<T>(rawItems);
        this.resultsFunc = resultsFunc;
    }

    protected RegressionBase(IEnumerable<T> items, Func<T, TResult?> resultFunc) :
        this(items, i => resultFunc(i).Value)
    {
        Filter.AddFilter(resultFunc);
    }

    protected IList<RegressionVariableDecl> IndependantVariables { get; } = new List<RegressionVariableDecl>();

    protected UnknownFilter<T> Filter { get; }

    protected double[][] InputData()
    {
        return Filter.FilteredResult().Select(i => IndependantVariables.Select(j => j.ValueFunc(i)).ToArray())
            .ToArray();
    }


    #region VariableMethods

    private TFinalType ReturnValue => (TFinalType)(object)this;

    public TFinalType WithVariable(Expression<Func<T, double?>> selector) => WithVariable(
        ExpressionPrinter.Print(selector), selector.Compile());

    public TFinalType WithVariable(string name, Func<T, double?> selector)
    {
        Filter.AddFilter(selector);
        return WithVariable(name, i => selector(i).Value);
    }

    public TFinalType WithVariable(Expression<Func<T, double>> selector) =>
        WithVariable(ExpressionPrinter.Print(selector), selector.Compile());

    public TFinalType WithVariable(string name, Func<T, double> selector)
    {
        IndependantVariables.Add(new RegressionVariableDecl(name, selector));
        return ReturnValue;
    }

    public TFinalType WithVariable(Expression<Func<T, bool>> selector) =>
        WithVariable(ExpressionPrinter.Print(selector), selector.Compile());

    public TFinalType WithVariable(string name, Func<T, bool> selector) =>
        WithVariable(name, i => selector(i) ? 1.0 : 0.0);

    public TFinalType WithVariable(Expression<Func<T, bool?>> selector) =>
        WithVariable(ExpressionPrinter.Print(selector), selector.Compile());

    public TFinalType WithVariable(string name, Func<T, bool?> selector)
    {
        Filter.AddFilter(selector);
        return WithVariable(name, i => selector(i).Value);
    }

    public TFinalType WithDummyVariable<TVariable>(Expression<Func<T, TVariable?>> selector,
        TVariable? referrant = null) where TVariable : struct =>
        WithDummyVariable(ExpressionPrinter.Print(selector), selector.Compile(), referrant);

    public TFinalType WithDummyVariable<TVariable>(string name, Func<T, TVariable?> selector,
        TVariable? referrant = null) where TVariable : struct
    {
        Filter.AddFilter(selector);
        return InnerWithDummyVariable(name, selector, referrant);
    }

    public TFinalType WithDummyVariable<TVariable>(Expression<Func<T, TVariable>> selector, TVariable? referrant = null,
        RequireStruct<TVariable> reserved = null) where TVariable : struct =>
        WithDummyVariable(ExpressionPrinter.Print(selector), selector.Compile(), referrant, reserved);

    public TFinalType WithDummyVariable<TVariable>(string name, Func<T, TVariable> selector,
        TVariable? referrant = null, RequireStruct<TVariable> reserved = null) where TVariable : struct =>
        InnerWithDummyVariable(name, selector, referrant);

    public TFinalType WithDummyVariable<TVariable>(Expression<Func<T, TVariable>> selector, TVariable referrant = null,
        RequireClass<TVariable> reserved = null) where TVariable : class =>
        WithDummyVariable(ExpressionPrinter.Print(selector), selector.Compile(), referrant, reserved);

    public TFinalType WithDummyVariable<TVariable>(string name, Func<T, TVariable> selector, TVariable referrant = null,
        RequireClass<TVariable> reserved = null) where TVariable : class
    {
        Filter.AddFilter(selector);
        return InnerWithDummyVariable(name, selector, referrant);
    }

    private TFinalType InnerWithDummyVariable<TVariable>(string name, Func<T, TVariable> selector, object referrant)
    {
        var values = rawItems.Select(selector).Where(i => i != null).Distinct().OrderBy(i => i);
        referrant = referrant ?? values.FirstOrDefault();
        SetupDummyVariable(name, selector, values.Where(i => !i.Equals(referrant)));
        return ReturnValue;
    }

    private void SetupDummyVariable<TVariable>(string name, Func<T, TVariable> selector, IEnumerable<TVariable> values)
    {
        foreach (var value in values)
        {
            var capturedValue = value;
            WithVariable($"{name}: {value}", i => selector(i).Equals(capturedValue) ? 1.0 : 0.0);
        }
    }

    #endregion

    #region RegressionVariableDecl

    public class RegressionVariableDecl
    {
        public string Name { get; }
        public Func<T, double> ValueFunc { get; }

        public RegressionVariableDecl(string name, Func<T, double> valueFunc)
        {
            Name = name;
            ValueFunc = valueFunc;
        }
    }

    #endregion

    protected string[] GetInputNames() => IndependantVariables.Select(i => i.Name).ToArray();
}