using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace Tevux.Software.Helpers;

/// <summary>
/// The only reason for reinventing the wheel is misbehaving apps, like Parsec, who likes blocking Clipboard a lot.
/// Thus, creating this class which does not throw exceptions and retries everything before complaining.
/// https://stackoverflow.com/questions/930219/how-to-handle-a-blocked-clipboard-and-other-oddities
/// </summary>
public class RobustClipboard {
    public static bool TryGetData(string format, out object data) {
        return TryGetData(format, out data, out _);
    }

    public static bool TryGetData(string format, out object data, out string errorMessage) {
        if (string.IsNullOrEmpty(format)) {
            errorMessage = "Format must be a valid string.";
            data = new object();
            return false;
        }

        var isSuccessful = false;
        try {
            data = Clipboard.GetData(format);
            isSuccessful = true;
            errorMessage = "";
        } catch (COMException) {
            // Yeah, no...
            data = new object();
            var hwnd = GetOpenClipboardWindow();
            var stringBuilder = new StringBuilder(501);
            GetWindowText(hwnd.ToInt32(), stringBuilder, 500);
            errorMessage = $"Program {stringBuilder} is preventing access to Clipboard.";
            Debug.WriteLine(errorMessage);
        }

        return isSuccessful;
    }

    public static bool TryGetText(out string text) {
        return TryGetText(TextDataFormat.Text, out text, out _);
    }

    public static bool TryGetText(out string text, out string errorMessage) {
        return TryGetText(TextDataFormat.Text, out text, out errorMessage);
    }

    public static bool TryGetText(TextDataFormat format, out string text, out string errorMessage) {
        var isSuccessful = false;

        try {
            text = Clipboard.GetText(format);
            isSuccessful = true;
            errorMessage = "";
        } catch (COMException) {
            // Yeah, no...
            text = "";
            var hwnd = GetOpenClipboardWindow();
            var stringBuilder = new StringBuilder(501);
            GetWindowText(hwnd.ToInt32(), stringBuilder, 500);
            errorMessage = $"Program {stringBuilder} is preventing access to Clipboard.";
            Debug.WriteLine(errorMessage);
        }

        return isSuccessful;
    }

    public static bool TrySetData(string format, object data) {
        return TrySetData(format, data, out _);
    }

    public static bool TrySetData(string format, object data, out string errorMessage) {
        if (string.IsNullOrEmpty(format)) {
            errorMessage = "Format must be a valid string.";
            return false;
        }

        var isSuccessful = false;
        try {
            Clipboard.SetData(format, data);
            isSuccessful = true;
            errorMessage = "";
        } catch (COMException) {
            // Yeah, no...
            var hwnd = GetOpenClipboardWindow();
            var stringBuilder = new StringBuilder(501);
            GetWindowText(hwnd.ToInt32(), stringBuilder, 500);
            errorMessage = $"Program {stringBuilder} is preventing access to Clipboard.";
            Debug.WriteLine(errorMessage);
        }

        return isSuccessful;
    }

    public static bool TrySetText(string text) {
        return TrySetText(TextDataFormat.Text, text, out _);
    }

    public static bool TrySetText(string text, out string errorMessage) {
        return TrySetText(TextDataFormat.Text, text, out errorMessage);
    }

    public static bool TrySetText(TextDataFormat format, string text, out string errorMessage) {
        var isSuccessful = false;
        try {
            Clipboard.SetData(DataFormats.UnicodeText, text);
            isSuccessful = true;
            errorMessage = "";
        } catch (COMException) {
            // Yeah, no...
            var hwnd = GetOpenClipboardWindow();
            var stringBuilder = new StringBuilder(501);
            GetWindowText(hwnd.ToInt32(), stringBuilder, 500);
            errorMessage = $"Program {stringBuilder} is preventing access to Clipboard.";
            Debug.WriteLine(errorMessage);

        }

        return isSuccessful;
    }

    [DllImport("user32.dll")]
    static extern IntPtr CloseClipboard();

    [DllImport("user32.dll")]
    static extern IntPtr GetOpenClipboardWindow();

    [DllImport("user32.dll")]
    static extern int GetWindowText(int hwnd, StringBuilder text, int count);
}
