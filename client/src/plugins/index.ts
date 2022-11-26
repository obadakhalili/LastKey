import { VueQueryPlugin } from "@tanstack/vue-query"

import router from "./vue-router"
import vuetify from "./vuetify"
import type { App } from "vue"

export function registerPlugins(app: App) {
  app.use(router)
  app.use(VueQueryPlugin)
  app.use(vuetify)
}
