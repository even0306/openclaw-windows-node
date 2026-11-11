using System.IO;

namespace OpenClaw.Tray.Tests;

/// <summary>
/// Pins the composer's draft binding. The chat composer must bind its TextBox
/// through AutomationTextInput's immediate native-property binding with a lazy
/// value reader: Reactor's own value/onChanged binding defers and
/// echo-suppresses change callbacks, so a miscounted suppression token can
/// swallow a real keystroke and let the next render write the stale draft back
/// over the edit (the caret jumps to the first column while typing).
/// </summary>
public sealed class ComposerDraftBindingTests
{
    private static string ReadSource(params string[] parts) =>
        File.ReadAllText(Path.Combine(TestRepositoryPaths.GetRepositoryRoot(), Path.Combine(parts)));

    [Fact]
    public void Composer_BindsDraftThroughLazyNativeBinding()
    {
        var src = ReadSource("src", "OpenClaw.Tray.WinUI", "Chat", "ReactorChatComposer.cs");
        Assert.Contains("AutomationTextInput.TextBox(", src);
        Assert.Contains("() => vm.Draft", src);
    }

    [Fact]
    public void TextInputBinding_ReadsValueAtApplyTimeAndPreservesCaret()
    {
        var src = ReadSource("src", "OpenClaw.Tray.WinUI", "Automations", "AutomationTextInput.cs");
        // Lazy reader: the owner value is fetched when Reactor applies the
        // binding, so a stale render capture can never reach the control.
        Assert.Contains("Func<string> valueOf", src);
        Assert.Contains("binding.Update(valueOf(), changed)", src);
        // Programmatic writes must restore the selection instead of resetting
        // the caret to the first column.
        Assert.Contains("WritePreservingCaret", src);
        Assert.Contains("SelectionStart", src);
    }
}
