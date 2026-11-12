using OpenClawTray.Presentation;

namespace OpenClaw.Tray.Tests.Presentation;

public sealed class WorkspaceSessionSectionsTests
{
    private static WorkspaceSession Session(string key, string? group = null) =>
        new(key, key, "main", Group: group);

    [Fact]
    public void Build_NoGroups_KeepsSingleFlatSection()
    {
        var sessions = new[] { Session("a"), Session("b") };

        var sections = WorkspaceSessionSections.Build(sessions, Array.Empty<string>());

        var section = Assert.Single(sections);
        Assert.Null(section.GroupName);
        Assert.Equal(new[] { "a", "b" }, section.Sessions.Select(s => s.Key));
    }

    [Fact]
    public void Build_Grouped_OrdersCatalogGroupsThenUnknownGroups()
    {
        var sessions = new[]
        {
            Session("u"),
            Session("z1", "Zeta"),
            Session("b1", "Beta"),
            Session("a1", "Alpha"),
            Session("a2", "Alpha"),
        };

        var sections = WorkspaceSessionSections.Build(sessions, new[] { "Beta", "Alpha" });

        Assert.Equal(
            new string?[] { null, "Beta", "Alpha", "Zeta" },
            sections.Select(s => s.GroupName));
        Assert.Equal(new[] { "u" }, sections[0].Sessions.Select(s => s.Key));
        Assert.Equal(new[] { "a1", "a2" }, sections[2].Sessions.Select(s => s.Key));
    }

    [Fact]
    public void Build_CatalogGroupsWithoutRows_AreOmitted()
    {
        var sections = WorkspaceSessionSections.Build(
            new[] { Session("a", "Live") }, new[] { "Empty", "Live" });

        Assert.Equal(new string?[] { null, "Live" }, sections.Select(s => s.GroupName));
    }

    [Fact]
    public void Build_Grouped_UngroupedSectionKeepsArrivalOrder()
    {
        var sessions = new[] { Session("u2"), Session("g", "G"), Session("u1") };

        var sections = WorkspaceSessionSections.Build(sessions, new[] { "G" });

        Assert.Equal(new[] { "u2", "u1" }, sections[0].Sessions.Select(s => s.Key));
    }
}
