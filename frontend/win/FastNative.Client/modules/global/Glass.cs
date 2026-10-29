using Windows.UI;

namespace FastNative.Client.Modules.Global;

public static class Glass
{
    public static Color Page(Theme theme) => theme == Theme.Dark
        ? Color.FromArgb(0xFF, 0x18, 0x18, 0x1B)
        : Color.FromArgb(0xFF, 0xFA, 0xFA, 0xFA);

    public static Color Chrome(Theme theme) => theme == Theme.Dark
        ? Color.FromArgb(0x73, 0, 0, 0)
        : Color.FromArgb(0x8C, 255, 255, 255);

    public static Color Content(Theme theme) => theme == Theme.Dark
        ? Color.FromArgb(0x18, 255, 255, 255)
        : Color.FromArgb(0x73, 255, 255, 255);

    public static Color Text(Theme theme) => theme == Theme.Dark
        ? Color.FromArgb(0xFF, 255, 255, 255)
        : Color.FromArgb(0xFF, 0x18, 0x18, 0x1B);
}
