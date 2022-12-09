import { CapacitorConfig } from "@capacitor/cli"

const config: CapacitorConfig = {
  appId: "net.azurewebsites.lastkey",
  server: {
    hostname: "lastkey.azurewebsites.net",
    androidScheme: "https",
  },
  appName: "LastKey",
  webDir: "dist",
  bundledWebRuntime: false,
  plugins: {
    CapacitorHttp: {
      enabled: true,
    },
  },
}

export default config
