package fastnative.modules.global

data class NavItem(val href: String, val label: String)

object ShellNav {
    val primary = listOf(
        NavItem("/", "Home"),
        NavItem("/sample/notes", "Sample notes"),
        NavItem("/admin", "Admin")
    )

    fun title(route: String): String = when (route) {
        "/" -> "Home"
        "/login" -> "Sign in"
        "/sample/notes" -> "Sample notes"
        "/admin" -> "Admin"
        else -> route
    }

    fun note(route: String): String = when (route) {
        "/" -> "Dashboard home. The web kit renders the same path after you run web use."
        "/login" -> "Sign in against the FastAPI auth routes."
        "/sample/notes" -> "Canonical sample notes module."
        "/admin" -> "Admin area."
        else -> "This path is not in client-routes.json."
    }

    fun isActive(route: String, item: NavItem): Boolean =
        if (item.href == "/") route == "/" else route == item.href || route.startsWith(item.href + "/")
}
