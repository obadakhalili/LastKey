import vue from "@vitejs/plugin-vue"
import vuetify from "vite-plugin-vuetify"

import { defineConfig } from "vite"
import { fileURLToPath, URL } from "node:url"

export default defineConfig({
  plugins: [vue(), vuetify({})],
  define: { "process.env": {} },
  resolve: {
    alias: {
      "@": fileURLToPath(new URL("./src", import.meta.url)),
    },
  },
  server: {
    port: 8080,
    proxy: {
      "/api": {
        target: "https://lastkey.azurewebsites.net/",
        changeOrigin: true,
      },
    },
  },
})
