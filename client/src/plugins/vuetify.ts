import "@mdi/font/css/materialdesignicons.css"
import "vuetify/styles"

import { createVuetify } from "vuetify"

const vuetify = createVuetify({
  theme: {
    themes: {
      light: {
        colors: {
          primary: "#1867C0",
          secondary: "#5CBBF6",
        },
      },
    },
  },
})

export default vuetify
