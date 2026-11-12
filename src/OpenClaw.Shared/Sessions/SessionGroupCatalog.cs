namespace OpenClaw.Shared.Sessions;

/// <summary>One custom sidebar group as reported by <c>sessions.groups.list</c>.</summary>
public sealed record SessionGroupInfo(string Name, int Position);

/// <summary>
/// The gateway's custom sidebar group catalog: ordered group names plus the raw
/// sidebar <c>sectionOrder</c> (which may interleave non-group sections). An
/// empty catalog means "no groups defined", including on gateways that predate
/// <c>sessions.groups.list</c>.
/// </summary>
public sealed class SessionGroupCatalog
{
    public static SessionGroupCatalog Empty { get; } =
        new(Array.Empty<SessionGroupInfo>(), Array.Empty<string>());

    public SessionGroupCatalog(IReadOnlyList<SessionGroupInfo> groups, IReadOnlyList<string> sectionOrder)
    {
        Groups = groups;
        SectionOrder = sectionOrder;
        GroupNames = groups
            .OrderBy(group => group.Position)
            .Select(group => group.Name)
            .ToArray();
    }

    public IReadOnlyList<SessionGroupInfo> Groups { get; }
    /// <summary>Raw sidebar section order; passed through unchanged on <c>sessions.groups.put</c>.</summary>
    public IReadOnlyList<string> SectionOrder { get; }
    /// <summary>Group names in sidebar display order.</summary>
    public IReadOnlyList<string> GroupNames { get; }
}
