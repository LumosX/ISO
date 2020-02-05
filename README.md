# ISO: InlinedScriptableObjects

An improved and extended version of Tom Kail's [ExtendedScriptableObjectDrawer](https://gist.github.com/tomkail/ba4136e6aa990f4dc94e0d39ec6a058c).

![image](https://user-images.githubusercontent.com/17273782/73863461-05bf1a00-4838-11ea-8c88-e73480c8690e.png)

(Reorderable lists and dictionaries can be found [here](https://github.com/LumosX/Unity-Reorderable-Collections))

## Why? (New features)
Because the original implementation has significant drawbacks, such as breaking completely when nested, and overrides the default drawer for all scriptable objects, which isn't necessarily something you always want to be doing. This version fixes the drawer's problems, makes the "inlining" optional, and includes an attribute that allows you to draw any ScriptableObject as inlined on a one-off basis, and optionally to customise the appearance (label) of your inlined object.

## How to use
Have your classes inherit from `InlinedScriptableObject` and that's it. Alternatively, decorate a member (which can either inherit from a `ScriptableObject` or an `InlinedScriptableObject`) with the `DrawInline` attribute. **Look at the examples in the repo.**
