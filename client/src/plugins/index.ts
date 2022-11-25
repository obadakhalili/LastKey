import { VueQueryPlugin } from "@tanstack/vue-query"

import vueRouter from "./vue-router"
import vuetify from "./vuetify"
import type { App } from "vue"

export function registerPlugins(app: App) {
  app.use(vueRouter)
  app.use(VueQueryPlugin)
  app.use(vuetify)
}
