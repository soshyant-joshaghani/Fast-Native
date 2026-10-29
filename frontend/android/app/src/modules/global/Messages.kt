package fastnative.modules.global

enum class Theme { Dark, Light }

object Messages {
    const val appName = "Fast Native"

    fun t(key: String): String = when (key) {
        "header_sign_in" -> "Sign in"
        "header_sign_out" -> "Sign out"
        "header_theme" -> "Theme"
        else -> key
    }
}
