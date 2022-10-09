# Unity UI Optimized
Rewritten Unity UI package (also known as uGUI) to make it more performant

- Install [by Git URL](https://docs.unity3d.com/Manual/upm-ui-giturl.html)

# WIP
This package is in Development, but is stable to use in production, more optimizations will appear time after time

## Optimizations
- RectMask2D Culling & Softness can be disabled | [test results]( https://github.com/mitay-walle/Unity3d-RectMask2DCulling/blob/main/README.md )
- LayoutRebuilder.Pool List<T> + HashSet<T> instead of Stack<T> 
