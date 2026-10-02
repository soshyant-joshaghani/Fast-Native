namespace FastNative.Client.Modules.Global;

public static class Messages
{
    public const string AppName = "Fast Native";

    public static string T(string key) => key switch
    {
        "header_sign_in" => "Sign in",
        "header_sign_out" => "Sign out",
        "header_theme" => "Theme",
        _ => key
    };
}
