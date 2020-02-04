using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using ZeroVector.Common.Reorderable;


namespace ZeroVector.Common.ISO {
    public class ExampleComponent : MonoBehaviour {

        [Header("Basics (read the tooltips)")]
        [Tooltip("Object inheriting from InlinedScriptableObject; drawn inline \"natively\"")]
        public ExampleInlinedSO inlinedObject;
        
        [Tooltip("Object inheriting from just ScriptableObject; drawn inline using the DrawInline attribute")]
        [DrawInline] public ExampleSO attributeInlinedObject;
        
        
        [Space]
        [Header("Label customisation")]
        [DrawInline(showName: false)] public ExampleInlinedSO inlinedObjNoLabel;
        
        [DrawInline(nameOverride: "Overridden label")] public ExampleInlinedSO inlinedObjOverrideLabel;
        
        
        [Space] 
        [Header("Nesting")]
        
        // If you have ReorderableCollections installed (https://github.com/LumosX/Unity-Reorderable-Collections),
        // you can uncomment the following, just for fun:
        //
        // public DictSO scriptableObjectDictionary;
        // // ReSharper disable once InconsistentNaming
        // [Serializable]
        // public class DictSO : ReorderableDictionary<string, ExampleInlinedSO, DictSO.KeyValuePair> {
        //     public override string DeduplicateKey(string duplicateKey) {
        //         return "New Scriptable Object " + (1 + this.Count(x => x.Key.StartsWith("New Scriptable Object")));
        //     }
        //
        //     [Serializable]
        //     public new class KeyValuePair : ReorderableDictionary<string, ExampleInlinedSO, KeyValuePair>.KeyValuePair { }
        // }

        [Tooltip("Nesting works everywhere.")]
        public List<ExampleInlinedSO> scriptableObjectList;
    }
}