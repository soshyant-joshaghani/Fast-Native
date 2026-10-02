namespace FastNative.Client.Modules.Global;

public sealed record NavItem(string Href, string Label, string Glyph);

public static class ShellNav
{
    public static IReadOnlyList<NavItem> Primary { get; } =
    [
        new("/", "Home", "\uE80F"),
        new("/sample/notes", "Sample notes", "\uE8A5"),
        new("/admin", "Admin", "\uE716")
    ];

    public static string Title(string route) => route switch
    {
        "/" => "Home",
        "/login" => "Sign in",
        "/sample/notes" => "Sample notes",
        "/admin" => "Admin",
        _ => route
    };

    public static string Note(string route) => route switch
    {
        "/" => "Dashboard home. The web kit renders the same path after you run web use.",
        "/login" => "Sign in against the FastAPI auth routes.",
        "/sample/notes" => "Canonical sample notes module.",
        "/admin" => "Admin area.",
        _ => "This path is not in client-routes.json."
    };

    public static bool IsActive(string route, NavItem item) =>
        item.Href == "/" ? route == "/" : route == item.Href || route.StartsWith(item.Href + "/");
}
