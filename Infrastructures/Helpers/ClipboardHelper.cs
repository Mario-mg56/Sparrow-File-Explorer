
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using System.Threading.Tasks;

namespace DynamicFileExplorer.UI.Helpers;

public static class ClipboardHelper
{
    public static async Task CopyToClipboard(TopLevel topLevel, string text)
    {
        if (topLevel?.Clipboard == null || string.IsNullOrWhiteSpace(text))
            return;

        await topLevel.Clipboard.SetTextAsync(text);
    }
}