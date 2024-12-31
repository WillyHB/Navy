using System;
using System.Collections.Generic;
using Force.DeepCloner;

namespace Navy;

public abstract class FieldWatcher
{
    protected static readonly List<FieldWatcher> runningWatchers = new();

    protected abstract void GenericUpdate();

    public static void Update()
    {
        foreach (FieldWatcher watcher in runningWatchers)
        {
            watcher.GenericUpdate();
        }
    }
}

/// <summary>
/// Watches a Field for changes
/// </summary>
/// <typeparam name="T">The type of the field the FieldWatcher is watching</typeparam>
public class FieldWatcher<T> : FieldWatcher, IDisposable
{
    private readonly bool isValueType;
    public FieldWatcher(Func<T> func)
    {
        isValueType = typeof(T).IsValueType;

        runningWatchers.Add(this);

        this.func = func;

        startValue = Field;

    }

    /// <summary>
    /// Called when the field is changed. The EventArgs is the PREVIOUS field value, prior to the change;
    /// </summary>
    public event EventHandler<T> FieldChanged;

    public T Field => isValueType ? func.Invoke() : func.Invoke().DeepClone();

    private T startValue;
    private readonly Func<T> func;

    protected override void GenericUpdate()
    {
        if (!EqualityComparer<T>.Default.Equals(Field, startValue))
        {
            FieldChanged?.Invoke(this, startValue);
            startValue = Field;
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        FieldChanged = null;
    }

}