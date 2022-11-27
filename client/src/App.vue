<script setup lang="ts">
import { computed, onMounted, watch } from "vue"
import { RouterView, useRoute, useRouter } from "vue-router"

import { useAuth } from "./utils/composables"

const router = useRouter()
const route = useRoute()
const routeIsPrivate = computed(() => route.meta.private as boolean | undefined)
const { user, verifyTokenPayloadCookie } = useAuth()

watch([user, routeIsPrivate], ([user, routeIsPrivate]) => {
  if (routeIsPrivate === undefined) {
    return
  }

  if (user && !routeIsPrivate) {
    router.push({ name: "Home" })
  } else if (!user && routeIsPrivate) {
    router.push({ name: "Login" })
  }
})

router.beforeEach((to, from, next) => {
  if (to.meta.private === undefined) {
    return next()
  }

  if (to.meta.private && user.value === null) {
    return next({ name: "Login", replace: true })
  }

  if (!to.meta.private && user.value !== null) {
    return next({ name: "Home", replace: true })
  }

  next()
})

onMounted(() => {
  verifyTokenPayloadCookie()
})
</script>

<template>
  <v-app>
    <v-main>
      <router-view />
    </v-main>
  </v-app>
</template>
