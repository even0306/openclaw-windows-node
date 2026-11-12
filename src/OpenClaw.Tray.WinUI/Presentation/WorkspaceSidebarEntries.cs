namespace OpenClawTray.Presentation;

/// <summary>
/// The gateway's sidebar entries list (<c>ui.prefs.sidebarEntries</c>), the
/// shared order store used by the web UI's sidebar: mixed entry ids like
/// <c>route:dashboards</c> and <c>session:&lt;key&gt;</c> in display order.
/// Operations here mirror the web's <c>writeSidebarEntryAt</c> / unpin
/// semantics so all clients interoperate on the same server-side list.
/// </summary>
internal static class WorkspaceSidebarEntries
{
    public const string SessionPrefix = "session:";

    /// <summary>Session keys in the entries' display order (session entries only).</summary>
    public static List<string> SessionOrder(IReadOnlyList<string> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);
        var order = new List<string>(entries.Count);
        foreach (var entry in entries)
        {
            if (!entry.StartsWith(SessionPrefix, StringComparison.Ordinal))
                continue;
            var key = entry[SessionPrefix.Length..];
            if (key.Length > 0 && !order.Contains(key))
                order.Add(key);
        }
        return order;
    }

    /// <summary>
    /// Moves the dragged session's entry before or after the target session's
    /// entry (null target appends at the end), like the web's
    /// <c>writeSidebarEntryAt</c>. When the target has no entry yet it is ranked
    /// first so the drop lands where it was shown.
    /// </summary>
    public static List<string> Move(
        IReadOnlyList<string> entries, string draggedKey, string? targetKey, bool after = false)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentException.ThrowIfNullOrWhiteSpace(draggedKey);

        var draggedEntry = SessionPrefix + draggedKey;
        var next = new List<string>(entries.Count + 1);
        foreach (var entry in entries)
            if (!string.Equals(entry, draggedEntry, StringComparison.Ordinal))
                next.Add(entry);

        if (targetKey is null)
        {
            next.Add(draggedEntry);
            return next;
        }

        var targetEntry = SessionPrefix + targetKey;
        var index = next.IndexOf(targetEntry);
        if (index < 0)
        {
            next.Add(targetEntry);
            index = next.Count - 1;
        }
        else if (after)
        {
            index += 1;
        }
        next.Insert(index, draggedEntry);
        return next;
    }

    /// <summary>Removes the session's entry (the web's unpin), keeping every other entry.</summary>
    public static List<string> Remove(IReadOnlyList<string> entries, string key)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var entry = SessionPrefix + key;
        var next = new List<string>(entries.Count);
        foreach (var existing in entries)
            if (!string.Equals(existing, entry, StringComparison.Ordinal))
                next.Add(existing);
        return next;
    }
}
