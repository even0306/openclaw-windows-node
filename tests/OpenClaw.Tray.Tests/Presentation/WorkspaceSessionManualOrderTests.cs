using OpenClawTray.Presentation;

namespace OpenClaw.Tray.Tests.Presentation;

public sealed class WorkspaceSessionManualOrderTests
{
    [Fact]
    public void Apply_NullOrEmptyOrder_KeepsInputOrder()
    {
        var rows = new[] { "a", "b", "c" };

        Assert.Equal(rows, WorkspaceSessionManualOrder.Apply(rows, null, k => k));
        Assert.Equal(rows, WorkspaceSessionManualOrder.Apply(rows, Array.Empty<string>(), k => k));
    }

    [Fact]
    public void Apply_RankedRowsFirst_UnrankedKeepNaturalOrder()
    {
        var rows = new[] { "a", "b", "c", "d" };
        var saved = new[] { "c", "a" };

        var ordered = WorkspaceSessionManualOrder.Apply(rows, saved, k => k);

        // c (rank 0), a (rank 1), then unranked b, d in input order.
        Assert.Equal(new[] { "c", "a", "b", "d" }, ordered);
    }

    [Fact]
    public void Move_DragBeforeTarget_InsertsBefore()
    {
        var moved = WorkspaceSessionManualOrder.Move(
            new[] { "a", "b", "c" }, new[] { "a", "b", "c" }, draggedKey: "c", targetKey: "a");

        Assert.Equal(new[] { "c", "a", "b" }, moved);
    }

    [Fact]
    public void Move_DragAfterTarget_InsertsAfter()
    {
        var moved = WorkspaceSessionManualOrder.Move(
            new[] { "a", "b", "c" }, new[] { "a", "b", "c" }, draggedKey: "a", targetKey: "b", after: true);

        Assert.Equal(new[] { "b", "a", "c" }, moved);
    }

    [Fact]
    public void Move_NullTarget_AppendsAtEnd()
    {
        var moved = WorkspaceSessionManualOrder.Move(
            new[] { "a", "b", "c" }, new[] { "a", "b", "c" }, draggedKey: "a", targetKey: null);

        Assert.Equal(new[] { "b", "c", "a" }, moved);
    }

    [Fact]
    public void Move_HiddenSavedKeys_KeepRelativeOrderAfterVisible()
    {
        var moved = WorkspaceSessionManualOrder.Move(
            new[] { "x", "a", "b", "y" }, new[] { "a", "b" }, draggedKey: "b", targetKey: "a");

        Assert.Equal(new[] { "b", "a", "x", "y" }, moved);
    }

    [Fact]
    public void Move_TargetNotDisplayed_AppendsAtEnd()
    {
        var moved = WorkspaceSessionManualOrder.Move(
            null, new[] { "a", "b" }, draggedKey: "a", targetKey: "ghost");

        Assert.Equal(new[] { "b", "a" }, moved);
    }
}
