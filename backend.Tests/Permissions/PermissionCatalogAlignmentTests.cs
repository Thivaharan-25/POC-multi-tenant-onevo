namespace OnevoHr.Api.Tests.Permissions;

public class PermissionCatalogAlignmentTests
{
    [Fact]
    public void CanonicalMapFile_Exists_AndIsReadable()
    {
        var path = @"C:\onevoNew\OneVo-HR\developer-platform\modules\module-catalog-manager\phase-1-feature-permission-map.md";

        Assert.True(File.Exists(path), $"Canonical permission map not found at {path}");

        var content = File.ReadAllText(path);
        Assert.False(string.IsNullOrWhiteSpace(content));
    }
}
