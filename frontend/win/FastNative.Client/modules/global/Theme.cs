namespace FastNative.Client.Modules.Global;

public enum Theme
{
    Dark,
    Light
}

public static class ThemeStore
{
    public static Theme Current { get; set; } = Theme.Dark;
}
