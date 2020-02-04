using System;
using UnityEngine;
using ZeroVector.Common.Reorderable;


namespace ZeroVector.Common.ISO {
    // ReSharper disable once InconsistentNaming
    public class ExampleSO : ScriptableObject {
        public float floatVal;
        public Quaternion quaternion;
        public Vector4 vector4;
        
        // If you have ReorderableCollections installed (https://github.com/LumosX/Unity-Reorderable-Collections),
        // you can uncomment everything else, just for fun:
        //
        // public QuatList quaternions;
        //
        // [Serializable] public class QuatList : ReorderableList<Quaternion> { }   
    }
}