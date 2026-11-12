using OpenClawTray.Presentation;

namespace OpenClaw.Tray.Tests.Presentation;

public sealed class WorkspaceSidebarEntriesTests
{
    [Fact]
    public void SessionOrder_ReadsOnlySessionEntriesInOrder()
    {
        var entries = new[] { "route:dashboards", "session:b", "plugin:x", "session:a", "session:b" };

        var order = WorkspaceSidebarEntries.SessionOrder(entries);

        Assert.Equal(new[] { "b", "a" }, order);
    }

    [Fact]
    public void Move_InsertsBeforeTargetEntry()
    {
        var moved = WorkspaceSidebarEntries.Move(
            new[] { "route:pages", "session:a", "session:b" }, draggedKey: "b", targetKey: "a");

        Assert.Equal(new[] { "route:pages", "session:b", "session:a" }, moved);
    }

    [Fact]
    public void Move_InsertsAfterTargetEntry()
    {
        var moved = WorkspaceSidebarEntries.Move(
            new[] { "session:a", "session:b" }, draggedKey: "a", targetKey: "b", after: true);

        Assert.Equal(new[] { "session:b", "session:a" }, moved);
    }

    [Fact]
    public void Move_NullTargetAppendsAtEnd()
    {
        var moved = WorkspaceSidebarEntries.Move(
            new[] { "session:a", "session:b" }, draggedKey: "a", targetKey: null);

        Assert.Equal(new[] { "session:b", "session:a" }, moved);
    }

    [Fact]
    public void Move_TargetWithoutEntry_RanksTargetFirst()
    {
        // Precise drop: the target is ranked too, so the row lands where shown.
        var moved = WorkspaceSidebarEntries.Move(
            new[] { "route:pages", "session:a" }, draggedKey: "a", targetKey: "c");

        Assert.Equal(new[] { "route:pages", "session:a", "session:c" }, moved);
    }

    [Fact]
    public void Move_KeepsNonSessionEntries()
    {
        var moved = WorkspaceSidebarEntries.Move(
            new[] { "route:pages", "person:me", "session:a" }, draggedKey: "a", targetKey: null);

        Assert.Equal(new[] { "route:pages", "person:me", "session:a" }, moved);
    }

    [Fact]
    public void Remove_KeepsEveryOtherEntry()
    {
        var next = WorkspaceSidebarEntries.Remove(
            new[] { "route:pages", "session:a", "session:b" }, "a");

        Assert.Equal(new[] { "route:pages", "session:b" }, next);
    }
}
