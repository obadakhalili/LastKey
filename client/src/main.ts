import { createApp } from "vue"

import { registerPlugins } from "@/plugins"
import App from "@/App.vue"
import "./styles.css"

const app = createApp(App)

registerPlugins(app)

app.mount("#app")
