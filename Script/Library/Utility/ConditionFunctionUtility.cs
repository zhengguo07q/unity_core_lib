using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


class ConditionFunctionUtility : SingletonMono<ConditionFunctionUtility>
{

    private Dictionary<Func<object, bool>, Func<object, bool>> funcs = new Dictionary<Func<object, bool>, Func<object, bool>>();

    public void AddFunction(Func<object, bool> condition, Func<object, bool> execution)
    {
        funcs.Add(condition, execution);
    }


    void Update()
    {
        List<Func<object, bool>> removeList = new List<Func<object, bool>>();
        var funcsEt = funcs.GetEnumerator();

        //foreach(KeyValuePair<Func<object, bool>, Func<object, bool>> pair in funcs)
        while(funcsEt.MoveNext())
        {
            KeyValuePair<Func<object, bool>, Func<object, bool>> pair = funcsEt.Current;
            Func<object, bool> key = pair.Key;
            Func<object, bool> value = pair.Value;
            bool funcReturn = key(null);
            Debug.LogError("funcReturn " + funcReturn);
            if (funcReturn)
            {
                value(funcReturn);
                removeList.Add(key);
            }
        }
        
        for (int i = 0, count = removeList.Count; i < count;i++ )
        {
            funcs.Remove(removeList[i]);
        }
    }

}

