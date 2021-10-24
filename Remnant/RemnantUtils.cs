﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;

namespace WaspPile.Remnant
{
    internal static class RemnantUtils
    {
        const BindingFlags allContextsInstance = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
        const BindingFlags allContextStatic = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        const BindingFlags allContextsCtor = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance;

        internal static void SetKey<tKey, tValue> (this Dictionary<tKey, tValue> dict, tKey key, tValue val){
            if (dict == null) throw new ArgumentNullException();
            if (!dict.ContainsKey(key)) dict.Add(key, val);
            else dict[key] = val;
        }
        internal static void TryRemoveKey<tKey, tVal> (this Dictionary<tKey, tVal> dict, tKey key)
        {
            if (dict.ContainsKey(key)) dict.Remove(key);
        }

        internal static RainWorld CRW => UnityEngine.Object.FindObjectOfType<RainWorld>();
    }
}
