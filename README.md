# Unity UI Optimized
Rewritten Unity UI package (also known as uGUI) to make it more performant

# WIP
This package is in Development, not finished solution

## Optimizations
- RectMask2D Culling & Softness is disabled [test results ]( https://github.com/mitay-walle/Unity3d-RectMask2DCulling/blob/main/README.md )
- LayoutRebuilder.Pool List<T> + HashSet<T> instead of Stack<T>
