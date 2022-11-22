import { defineConfig } from "vite"
import vue from "@vitejs/plugin-vue"
import vueJsx from "@vitejs/plugin-vue-jsx"
import { VitePWA } from "vite-plugin-pwa"

export default defineConfig({
  plugins: [
    vue(),
    vueJsx(),
    VitePWA({
      injectRegister: "script",
      includeAssets: ["images/favicon-32x32.png"],
      manifest: {
        name: "LastKey",
        short_name: "LastKey",
        description: "Digital Unlocking Assistant",
        theme_color: "#ffffff",
        icons: [
          {
            src: "images/pwa-192x192.png",
            sizes: "192x192",
            type: "image/png",
          },
          {
            src: "images/pwa-512x512.png",
            sizes: "512x512",
            type: "image/png",
          },
          {
            src: "images/pwa-512x512.png",
            sizes: "512x512",
            type: "image/png",
            purpose: "any maskable",
          },
        ],
      },
    }),
  ],
})
