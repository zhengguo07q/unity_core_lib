// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: ListCollection.cs
//  Creator 	: panyuhuan
//  Date		: 2016-9-21
//  Comment		:
// ***************************************************************


using SLua;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;


[CustomLuaClass]
public class ListCollection : ArrayList
{
    public CompareFunc OnCompareFunc;
    public event DataChangeHandler OnAddItem;
    public event DataChangeHandler OnRemoveItem;
    public event DataChangeHandler OnUpdateItem;
    public event DataChangeHandler OnClearItem;

    public ListCollection():base()
    {
    }


    public ListCollection(ICollection c):base(c)
    {

    }


    public ListCollection(LuaTable table)
    {
        IEnumerator<LuaTable.TablePair> tablePair = table.GetEnumerator();
        while (tablePair.MoveNext())
        {
            object value = tablePair.Current.value;
            if (value != null)
            {
                Add(value);
            }
        }
    }


    public object GetListByIndex(int index)
    {
        return this[index];
    }


    public new int Add(object value)
    {
        int result = base.Add(value);
        if (OnAddItem != null)
            OnAddItem(value);
        return result;
    }


    public new void RemoveAt(int index)
    {
        object removeItem = this[index];
        base.RemoveAt(index);
        if (OnRemoveItem != null)
            OnRemoveItem(removeItem);
    }


    public new void Remove(object obj)
    {
        base.Remove(obj);
        if (OnRemoveItem != null)
            OnRemoveItem(obj);
    }


    public new void Clear()
    {
        base.Clear();
        if (OnClearItem != null)
            OnClearItem(this);
    }


    public void UpdateItem(object value)
    {
        if (OnUpdateItem != null && value != null)
            OnUpdateItem(value);
    }

    /// <summary>
    /// List.Sort equivalent. Manual sorting causes no GC allocations.
    /// </summary>

    [DebuggerHidden]
    [DebuggerStepThrough]
    public void Sort (CompareFunc comparer)
    {
        OnCompareFunc = comparer;
        int start = 0;
        int max = Count - 1;
        bool changed = true;
        
        while (changed)
        {
            changed = false;
            
            for (int i = start; i < max; ++i)
            {
                // Compare the two values
                if (comparer(this[i], this[i + 1]) > 0)
                {
                    // Swap the values
                    object temp = this[i];
                    this[i] = this[i + 1];
                    this[i + 1] = temp;
                    changed = true;
                }
                else if (!changed)
                {
                    // Nothing has changed -- we can start here next time
                    start = (i == 0) ? 0 : i - 1;
                }
            }
        }
    }
    

    /// <summary>
    /// Comparison function should return -1 if left is less than right, 1 if left is greater than right, and 0 if they match.
    /// </summary>
    
    public delegate int CompareFunc (object left, object right);
}

