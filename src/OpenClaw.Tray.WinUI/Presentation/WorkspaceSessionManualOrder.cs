namespace OpenClawTray.Presentation;

/// <summary>
/// Manual sidebar ordering from drag-to-reorder. The saved order is a list of
/// session keys in display order (client-local); rows without a saved rank keep
/// their natural relative order after the ranked rows.
/// </summary>
internal static class WorkspaceSessionManualOrder
{
    /// <summary>Stable sort applying the saved manual order.</summary>
    public static IReadOnlyList<T> Apply<T>(
        IReadOnlyList<T> rows, IReadOnlyList<string>? savedOrder, Func<T, string> keyOf)
    {
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(keyOf);
        if (savedOrder is not { Count: > 0 } || rows.Count < 2)
            return rows;

        var rank = new Dictionary<string, int>(StringComparer.Ordinal);
        for (var i = 0; i < savedOrder.Count; i++)
            rank.TryAdd(savedOrder[i], i);

        return rows
            .Select((row, index) => (row, index))
            .OrderBy(entry => rank.TryGetValue(keyOf(entry.row), out var r) ? r : int.MaxValue)
            .ThenBy(entry => entry.index)
            .Select(entry => entry.row)
            .ToArray();
    }

    /// <summary>
    /// Applies a drag move of <paramref name="draggedKey"/> before or after
    /// <paramref name="targetKey"/> (null appends at the end) over the current
    /// display order, and merges the result into the saved order. Keys that are
    /// saved but not currently displayed keep their relative order.
    /// </summary>
    public static List<string> Move(
        IReadOnlyList<string>? savedOrder,
        IReadOnlyList<string> displayOrder,
        string draggedKey,
        string? targetKey,
        bool after = false)
    {
        ArgumentNullException.ThrowIfNull(displayOrder);

        var next = new List<string>(displayOrder.Count);
        foreach (var key in displayOrder)
            if (!string.Equals(key, draggedKey, StringComparison.Ordinal))
                next.Add(key);

        var index = targetKey is null ? next.Count : next.IndexOf(targetKey);
        if (index < 0)
            index = next.Count;
        else if (after)
            index += 1;
        if (index > next.Count)
            index = next.Count;
        next.Insert(index, draggedKey);

        var visible = new HashSet<string>(displayOrder, StringComparer.Ordinal);
        var hidden = new List<string>();
        foreach (var key in savedOrder ?? Array.Empty<string>())
            if (!visible.Contains(key) && !string.Equals(key, draggedKey, StringComparison.Ordinal) && !hidden.Contains(key))
                hidden.Add(key);
        next.AddRange(hidden);
        return next;
    }
}
