namespace OpenClawTray.Presentation;

/// <summary>One sidebar section: a custom group (named) or the ungrouped tail (null name).</summary>
internal sealed record WorkspaceSessionSection(string? GroupName, IReadOnlyList<WorkspaceSession> Sessions);

/// <summary>
/// Pure projection of sidebar rows into group sections. Catalog groups come
/// first in catalog order; rows naming a group the catalog does not list are
/// kept in name order after them so a stale or just-created group never hides
/// rows. The ungrouped bucket trails the named groups like the web UI's
/// trailing "other sessions" section. When no grouping exists the result is a
/// single unnamed section, which keeps the flat sidebar layout unchanged.
/// </summary>
internal static class WorkspaceSessionSections
{
    public static IReadOnlyList<WorkspaceSessionSection> Build(
        IReadOnlyList<WorkspaceSession> sessions, IReadOnlyList<string> groupOrder)
    {
        ArgumentNullException.ThrowIfNull(sessions);
        ArgumentNullException.ThrowIfNull(groupOrder);

        var byGroup = new Dictionary<string, List<WorkspaceSession>>(StringComparer.Ordinal);
        var ungrouped = new List<WorkspaceSession>();
        foreach (var session in sessions)
        {
            if (session.Group is { Length: > 0 } group)
            {
                if (!byGroup.TryGetValue(group, out var bucket))
                    byGroup[group] = bucket = new List<WorkspaceSession>();
                bucket.Add(session);
            }
            else
            {
                ungrouped.Add(session);
            }
        }

        if (byGroup.Count == 0)
            return new[] { new WorkspaceSessionSection(null, ungrouped) };

        // Named groups lead in catalog order (the web UI's category zones), and
        // the ungrouped "other sessions" rows trail them like the web's trailing
        // ungrouped bucket.
        var sections = new List<WorkspaceSessionSection>(byGroup.Count + 1);
        foreach (var name in groupOrder)
        {
            if (byGroup.Remove(name, out var bucket))
                sections.Add(new WorkspaceSessionSection(name, bucket));
        }
        foreach (var (name, bucket) in byGroup.OrderBy(entry => entry.Key, StringComparer.Ordinal))
            sections.Add(new WorkspaceSessionSection(name, bucket));
        sections.Add(new WorkspaceSessionSection(null, ungrouped));
        return sections;
    }
}
