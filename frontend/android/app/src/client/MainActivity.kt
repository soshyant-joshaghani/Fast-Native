package fastnative.client

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import fastnative.modules.global.AppShell
import fastnative.modules.global.Theme

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContent {
            var route by remember { mutableStateOf("/") }
            var theme by remember { mutableStateOf(Theme.Dark) }
            var signedIn by remember { mutableStateOf(false) }
            AppShell(
                route = route,
                theme = theme,
                signedIn = signedIn,
                onNavigate = { route = it },
                onTheme = { theme = if (theme == Theme.Dark) Theme.Light else Theme.Dark },
                onSignedIn = { signedIn = it }
            )
        }
    }
}
