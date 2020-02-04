using System;
using UnityEngine;
using ZeroVector.Common.Reorderable;


namespace ZeroVector.Common.ISO {
    // ReSharper disable once InconsistentNaming
    public class ExampleInlinedSO : InlineScriptableObject {
        public int integer;
        public string text;
        public Vector3 vector;
        
        // If you have ReorderableCollections installed (https://github.com/LumosX/Unity-Reorderable-Collections),
        // you can uncomment everything else, just for fun:
        //
        // public Vector3List vectorList;
        //
        // [Serializable] public class Vector3List : ReorderableList<Vector3> { }
    }
}