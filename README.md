The reason I'm reinventing the wheel here is because some applications do not play fair when it comes to Clipboard usage, namely TeamViewer and Parsec. As of 2022, Parsec is extremely bad: it periodically block Clipboard access **for seconds** at a time, even if you don't do any copy-pasting! This is especially annoying, because:

    1. Windows API provides ways to share clipboard nicely between apps. 
    2. Windows API specifically states "Clipboard operation must only happen on direct user request" (can't find the exact quote, but it is there).

.NET even goes further and adds more internal synchronization objects so .NET applications essentially never disturb each other.

And thus, `RobustClipboard`. It wraps .NET methods with `try`/`catch` statements, and additionally, if Clipboard access is denied, figures out the offending application.

# Usage

Methods are pretty self explanatory. For example:

```
if (RobustClipboard.TrySetText("copy me", out var errorMessage) == false) {
    Debug.WriteLine($"Oh no, Clipboard access was blocked by {errorMessage}!");
};
```

