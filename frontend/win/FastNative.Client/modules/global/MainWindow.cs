using System.IO;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace FastNative.Client.Modules.Global;

public sealed class MainWindow : Window
{
    string route = "/";
    bool headerVisible = true;
    bool navVisible = true;
    bool signedIn;
    bool landscape = true;

    public MainWindow()
    {
        Title = Messages.AppName;
        var icon = Path.Combine(AppContext.BaseDirectory, "assets", "app.ico");
        if (File.Exists(icon))
            AppWindow.SetIcon(icon);
        SizeChanged += (_, args) =>
        {
            var wide = args.Size.Width >= args.Size.Height;
            if (wide == landscape) return;
            landscape = wide;
            Later(Show);
        };
        Show();
    }

    void Go(string next)
    {
        route = next;
        Later(Show);
    }

    void Later(Action action)
    {
        if (!DispatcherQueue.TryEnqueue(() => action())) action();
    }

    void Show()
    {
        Title = Messages.AppName;
        var theme = ThemeStore.Current;
        var root = new Grid
        {
            RequestedTheme = theme == Theme.Dark ? ElementTheme.Dark : ElementTheme.Light,
            Background = new SolidColorBrush(Glass.Page(theme))
        };
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        if (!landscape) root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        var header = BuildHeader(theme);
        Grid.SetRow(header, 0);
        root.Children.Add(header);

        var body = landscape ? LandscapeBody(theme) : ContentCard(theme);
        Grid.SetRow(body, 1);
        root.Children.Add(body);
        if (!landscape)
        {
            var nav = navVisible
                ? BuildNavigator(theme, verticalLayers: false)
                : ShowNavButton(theme, verticalLayers: false);
            Grid.SetRow(nav, 2);
            root.Children.Add(nav);
        }
        PaintText(root, theme);
        Content = root;
    }

    static void PaintText(DependencyObject node, Theme theme)
    {
        var ink = new SolidColorBrush(Glass.Text(theme));
        var mode = theme == Theme.Dark ? ElementTheme.Dark : ElementTheme.Light;
        if (node is FrameworkElement element) element.RequestedTheme = mode;
        switch (node)
        {
            case TextBlock text:
                text.Foreground = ink;
                break;
            case IconElement icon:
                icon.Foreground = ink;
                break;
            case Control control:
                control.Foreground = ink;
                break;
        }

        switch (node)
        {
            case Panel panel:
                foreach (var child in panel.Children)
                    if (child is DependencyObject item) PaintText(item, theme);
                break;
            case Border border when border.Child is DependencyObject borderChild:
                PaintText(borderChild, theme);
                break;
            case ContentControl host when host.Content is DependencyObject content:
                PaintText(content, theme);
                break;
        }
    }

    FrameworkElement LandscapeBody(Theme theme)
    {
        var row = new Grid();
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        var nav = navVisible ? BuildNavigator(theme, verticalLayers: true) : ShowNavButton(theme, verticalLayers: true);
        Grid.SetColumn(nav, 0);
        row.Children.Add(nav);
        var card = ContentCard(theme);
        Grid.SetColumn(card, 1);
        row.Children.Add(card);
        return row;
    }

    FrameworkElement BuildHeader(Theme theme)
    {
        var chrome = new SolidColorBrush(Glass.Chrome(theme));
        var wrap = new StackPanel { Margin = new Thickness(8, 8, 8, 0) };
        if (headerVisible)
        {
            var bar = new Grid
            {
                Height = 48,
                Background = chrome,
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(8, 0, 8, 0)
            };
            bar.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            bar.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            var home = ChromeButton(ShellNav.Title(route), () => Go("/"));
            home.HorizontalAlignment = HorizontalAlignment.Left;
            home.FontWeight = Microsoft.UI.Text.FontWeights.SemiBold;
            Grid.SetColumn(home, 0);
            bar.Children.Add(home);
            var actions = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 4, VerticalAlignment = VerticalAlignment.Center };
            actions.Children.Add(IconButton(theme == Theme.Dark ? "\uE706" : "\uE708", () =>
            {
                ThemeStore.Current = theme == Theme.Dark ? Theme.Light : Theme.Dark;
                Later(Show);
            }));
            if (signedIn)
                actions.Children.Add(IconButton("\uE8AC", () => { signedIn = false; Go("/"); }));
            else
                actions.Children.Add(ChromeButton(Messages.T("header_sign_in"), () => Go("/login")));
            Grid.SetColumn(actions, 1);
            bar.Children.Add(actions);
            wrap.Children.Add(bar);
        }
        var toggle = IconButton(headerVisible ? "\uE70E" : "\uE70D", () => { headerVisible = !headerVisible; Later(Show); });
        toggle.HorizontalAlignment = HorizontalAlignment.Center;
        toggle.MinWidth = 48;
        toggle.Height = 16;
        toggle.Padding = new Thickness(12, 0, 12, 0);
        toggle.Background = chrome;
        toggle.CornerRadius = new CornerRadius(0, 0, 8, 8);
        wrap.Children.Add(toggle);
        return wrap;
    }

    FrameworkElement BuildNavigator(Theme theme, bool verticalLayers)
    {
        var host = new StackPanel
        {
            Orientation = verticalLayers ? Orientation.Vertical : Orientation.Horizontal,
            Spacing = 4,
            Background = new SolidColorBrush(Glass.Chrome(theme)),
            Padding = new Thickness(4),
            CornerRadius = new CornerRadius(12)
        };
        foreach (var item in ShellNav.Primary) host.Children.Add(NavButton(item, theme));
        var hide = EdgeTab(verticalLayers ? "\uE76B" : "\uE70E", theme, verticalLayers, () => { navVisible = false; Later(Show); });
        var shell = new StackPanel
        {
            Orientation = verticalLayers ? Orientation.Horizontal : Orientation.Vertical,
            Spacing = 0,
            Margin = verticalLayers ? new Thickness(8, 0, 0, 0) : new Thickness(8, 0, 8, 8),
            VerticalAlignment = verticalLayers ? VerticalAlignment.Center : VerticalAlignment.Bottom,
            HorizontalAlignment = verticalLayers ? HorizontalAlignment.Left : HorizontalAlignment.Center
        };
        if (verticalLayers)
        {
            shell.Children.Add(host);
            shell.Children.Add(hide);
        }
        else
        {
            hide.CornerRadius = new CornerRadius(8, 8, 0, 0);
            hide.VerticalAlignment = VerticalAlignment.Bottom;
            shell.Children.Add(hide);
            shell.Children.Add(host);
        }
        return shell;
    }

    Button EdgeTab(string glyph, Theme theme, bool verticalLayers, Action click)
    {
        var button = new Button
        {
            Content = new FontIcon { Glyph = glyph, FontSize = 10 },
            Padding = new Thickness(0),
            BorderThickness = new Thickness(0),
            Background = new SolidColorBrush(Glass.Chrome(theme)),
            HorizontalAlignment = verticalLayers ? HorizontalAlignment.Left : HorizontalAlignment.Center,
            VerticalAlignment = verticalLayers ? VerticalAlignment.Center : VerticalAlignment.Top
        };
        if (verticalLayers)
        {
            button.Width = 16;
            button.MinHeight = 48;
            button.CornerRadius = new CornerRadius(0, 8, 8, 0);
        }
        else
        {
            button.Height = 16;
            button.MinWidth = 48;
            button.CornerRadius = new CornerRadius(8, 8, 0, 0);
        }
        button.Click += (_, _) => click();
        return button;
    }

    Button ShowNavButton(Theme theme, bool verticalLayers)
    {
        var show = EdgeTab(verticalLayers ? "\uE76C" : "\uE70D", theme, verticalLayers, () => { navVisible = true; Later(Show); });
        if (verticalLayers)
        {
            show.VerticalAlignment = VerticalAlignment.Center;
            show.Margin = new Thickness(8, 0, 0, 0);
            show.CornerRadius = new CornerRadius(0, 8, 8, 0);
        }
        else
        {
            show.HorizontalAlignment = HorizontalAlignment.Center;
            show.Margin = new Thickness(0, 0, 0, 8);
            show.CornerRadius = new CornerRadius(8, 8, 0, 0);
        }
        return show;
    }

    Button NavButton(NavItem item, Theme theme)
    {
        var label = new TextBlock
        {
            Text = item.Label,
            FontSize = 11,
            TextWrapping = TextWrapping.Wrap,
            TextAlignment = TextAlignment.Center,
            MaxWidth = 88
        };
        var icon = new FontIcon { Glyph = item.Glyph, FontSize = 16 };
        var stack = new StackPanel { Spacing = 2, HorizontalAlignment = HorizontalAlignment.Center };
        stack.Children.Add(icon);
        stack.Children.Add(label);
        var button = new Button
        {
            Content = stack,
            MinWidth = 72,
            Padding = new Thickness(4, 8, 4, 8),
            BorderThickness = new Thickness(0),
            Background = ShellNav.IsActive(route, item)
                ? new SolidColorBrush(Glass.Content(theme))
                : new SolidColorBrush(Microsoft.UI.Colors.Transparent),
            CornerRadius = new CornerRadius(8)
        };
        button.Click += (_, _) => Go(item.Href);
        return button;
    }

    FrameworkElement ContentCard(Theme theme)
    {
        var stack = new StackPanel { Spacing = 10 };
        stack.Children.Add(new TextBlock
        {
            Text = route == "/" ? Messages.AppName : ShellNav.Title(route),
            FontSize = 24,
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold
        });
        stack.Children.Add(new TextBlock
        {
            Text = ShellNav.Note(route),
            TextWrapping = TextWrapping.Wrap,
            Opacity = 0.85,
            MaxWidth = 560
        });
        if (route == "/login")
            stack.Children.Add(ChromeButton("Continue", () => { signedIn = true; Go("/"); }));
        return new Border
        {
            Child = stack,
            Margin = new Thickness(12),
            Padding = new Thickness(16),
            CornerRadius = new CornerRadius(16),
            Background = new SolidColorBrush(Glass.Content(theme)),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
    }

    static Button ChromeButton(string text, Action click)
    {
        var button = new Button
        {
            Content = text,
            Background = new SolidColorBrush(Microsoft.UI.Colors.Transparent),
            BorderThickness = new Thickness(0),
            Padding = new Thickness(10, 6, 10, 6)
        };
        button.Click += (_, _) => click();
        return button;
    }

    static Button IconButton(string glyph, Action click)
    {
        var button = new Button
        {
            Content = new FontIcon { Glyph = glyph, FontSize = 16 },
            Width = 36,
            Height = 36,
            Padding = new Thickness(0),
            Background = new SolidColorBrush(Microsoft.UI.Colors.Transparent),
            BorderThickness = new Thickness(0),
            CornerRadius = new CornerRadius(18)
        };
        button.Click += (_, _) => click();
        return button;
    }
}
