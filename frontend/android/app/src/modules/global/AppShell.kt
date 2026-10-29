package fastnative.modules.global

import android.app.Activity
import android.content.res.Configuration
import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.horizontalScroll
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.WindowInsets
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.safeDrawing
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.layout.windowInsetsPadding
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.verticalScroll
import androidx.compose.material3.Text
import androidx.compose.material3.TextButton
import androidx.compose.runtime.Composable
import androidx.compose.runtime.SideEffect
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.platform.LocalConfiguration
import androidx.compose.ui.platform.LocalView
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.text.style.TextOverflow
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.core.view.WindowCompat

@Composable
fun AppShell(
    route: String,
    theme: Theme,
    signedIn: Boolean,
    onNavigate: (String) -> Unit,
    onTheme: () -> Unit,
    onSignedIn: (Boolean) -> Unit
) {
    val landscape = LocalConfiguration.current.orientation == Configuration.ORIENTATION_LANDSCAPE
    val view = LocalView.current
    SideEffect {
        val window = (view.context as Activity).window
        val controller = WindowCompat.getInsetsController(window, view)
        val lightBars = theme != Theme.Dark
        controller.isAppearanceLightStatusBars = lightBars
        controller.isAppearanceLightNavigationBars = lightBars
    }
    Box(Modifier.fillMaxSize().background(Glass.background(theme))) {
        Column(
            Modifier
                .fillMaxSize()
                .windowInsetsPadding(WindowInsets.safeDrawing)
                .padding(8.dp)
        ) {
            Header(route, theme, signedIn, onNavigate, onTheme, onSignedIn)
            if (landscape) {
                Row(
                    Modifier.weight(1f).fillMaxWidth().padding(top = 8.dp),
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    Navigator(route, theme, vertical = true, onNavigate = onNavigate)
                    Content(route, theme, onNavigate, onSignedIn, Modifier.weight(1f).padding(start = 8.dp))
                }
            } else {
                Content(route, theme, onNavigate, onSignedIn, Modifier.weight(1f).padding(top = 8.dp))
                Navigator(
                    route,
                    theme,
                    vertical = false,
                    onNavigate = onNavigate,
                    modifier = Modifier.padding(top = 8.dp)
                )
            }
        }
    }
}

@Composable
private fun Header(
    route: String,
    theme: Theme,
    signedIn: Boolean,
    onNavigate: (String) -> Unit,
    onTheme: () -> Unit,
    onSignedIn: (Boolean) -> Unit
) {
    Row(
        Modifier
            .fillMaxWidth()
            .height(48.dp)
            .clip(RoundedCornerShape(12.dp))
            .background(Glass.chrome(theme))
            .padding(horizontal = 8.dp),
        verticalAlignment = Alignment.CenterVertically
    ) {
        Text(
            ShellNav.title(route),
            color = Glass.text(theme),
            fontWeight = FontWeight.SemiBold,
            maxLines = 1,
            overflow = TextOverflow.Ellipsis,
            modifier = Modifier.weight(1f).clickable { onNavigate("/") }
        )
        TextButton(onClick = onTheme) { Text(Messages.t("header_theme"), color = Glass.text(theme)) }
        if (signedIn) {
            TextButton(onClick = { onSignedIn(false); onNavigate("/") }) {
                Text(Messages.t("header_sign_out"), color = Glass.text(theme))
            }
        } else {
            TextButton(onClick = { onNavigate("/login") }) {
                Text(Messages.t("header_sign_in"), color = Glass.text(theme))
            }
        }
    }
}

@Composable
private fun Navigator(
    route: String,
    theme: Theme,
    vertical: Boolean,
    onNavigate: (String) -> Unit,
    modifier: Modifier = Modifier
) {
    val chrome = Modifier
        .clip(RoundedCornerShape(12.dp))
        .background(Glass.chrome(theme))
        .padding(4.dp)
        .then(modifier)
    if (vertical) {
        Column(
            chrome.width(96.dp).verticalScroll(rememberScrollState()),
            verticalArrangement = Arrangement.spacedBy(4.dp)
        ) {
            ShellNav.primary.forEach { item -> NavButton(route, item, theme, onNavigate) }
        }
    } else {
        Row(
            chrome.fillMaxWidth().horizontalScroll(rememberScrollState()),
            horizontalArrangement = Arrangement.spacedBy(4.dp, Alignment.CenterHorizontally)
        ) {
            ShellNav.primary.forEach { item -> NavButton(route, item, theme, onNavigate) }
        }
    }
}

@Composable
private fun NavButton(route: String, item: NavItem, theme: Theme, onNavigate: (String) -> Unit) {
    val active = ShellNav.isActive(route, item)
    Column(
        Modifier
            .width(88.dp)
            .clip(RoundedCornerShape(8.dp))
            .background(if (active) Glass.accent else Glass.content(theme))
            .clickable { onNavigate(item.href) }
            .padding(vertical = 8.dp, horizontal = 4.dp),
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        Text(
            item.label,
            color = Glass.text(theme),
            fontSize = 12.sp,
            textAlign = TextAlign.Center,
            maxLines = 2
        )
    }
}

@Composable
private fun Content(
    route: String,
    theme: Theme,
    onNavigate: (String) -> Unit,
    onSignedIn: (Boolean) -> Unit,
    modifier: Modifier
) {
    Column(
        modifier
            .fillMaxSize()
            .clip(RoundedCornerShape(16.dp))
            .background(Glass.content(theme))
            .padding(16.dp)
            .verticalScroll(rememberScrollState())
    ) {
        Text(ShellNav.title(route), color = Glass.text(theme), fontSize = 24.sp, fontWeight = FontWeight.SemiBold)
        Text(ShellNav.note(route), color = Glass.text(theme), modifier = Modifier.padding(top = 8.dp))
        if (route == "/login") {
            TextButton(onClick = { onSignedIn(true); onNavigate("/") }) {
                Text("Continue", color = Glass.text(theme))
            }
        }
    }
}
