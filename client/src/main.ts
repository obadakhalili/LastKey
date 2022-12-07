import { createApp } from "vue"
import axios from "axios"

import App from "./App.vue"
import { registerPlugins } from "./plugins"
import "./styles.css"

const app = createApp(App)

registerPlugins(app)

if (import.meta.env.VITE_BE_SERVER_URL) {
  axios.defaults.baseURL = import.meta.env.VITE_BE_SERVER_URL
  axios.defaults.withCredentials = true
}

app.mount("#app")
