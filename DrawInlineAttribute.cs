using UnityEngine;

namespace ZeroVector.Common.ISO {
    
    /// <summary>
    /// This attribute allows you to draw any scriptable object as inlined and/or to customise the appearance of an
    /// inlined scriptable object.
    /// </summary>
    public class DrawInlineAttribute : PropertyAttribute {
        
        /// <summary>
        /// Whether to display a label with the name of the object.
        /// </summary>
        public readonly bool showName = true;
        
        /// <summary>
        /// If set and not empty, will override the inlined object's label with the given string.
        /// </summary>
        public readonly string nameOverride = null;

        /// <summary>
        /// Allows you to draw any scriptable object as inlined, or to customise the appearance of
        /// an inlined scriptable object.
        /// </summary>
        public DrawInlineAttribute(bool showName = true, string nameOverride = "") {
            this.showName = showName;
            this.nameOverride = nameOverride;
        }
    }
}