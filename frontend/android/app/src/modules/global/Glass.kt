package fastnative.modules.global

import androidx.compose.ui.graphics.Color

object Glass {
    fun background(theme: Theme) = if (theme == Theme.Dark) Color(0xFF18181B) else Color(0xFFFAFAFA)
    fun chrome(theme: Theme) = if (theme == Theme.Dark) Color(0xFF27272A) else Color(0xFFFFFFFF)
    fun content(theme: Theme) = if (theme == Theme.Dark) Color(0xFF3F3F46) else Color(0xFFF4F4F5)
    fun text(theme: Theme) = if (theme == Theme.Dark) Color(0xFFFAFAFA) else Color(0xFF18181B)
    val accent = Color(0xFF0EA5E9)
}
