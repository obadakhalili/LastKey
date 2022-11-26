<script setup lang="ts">
import { computed, onMounted, watch } from "vue"
import { RouterView, useRoute, useRouter } from "vue-router"

import { useAuth } from "./utils/composables"

const router = useRouter()
const route = useRoute()
const isPrivateRoute = computed(() => route.meta.private as boolean | undefined)
const { user, verifyTokenPayloadCookie } = useAuth()

watch(
  [user, isPrivateRoute],
  ([user, isPrivateRoute]) => {
    if (isPrivateRoute === undefined) {
      return
    }

    if (user && !isPrivateRoute) {
      router.replace({ name: "Home" })
    } else if (user === null && isPrivateRoute) {
      router.replace({ name: "Login" })
    }
  },
  { immediate: true },
)

onMounted(() => {
  verifyTokenPayloadCookie()
})
</script>

<template>
  <v-app>
    <v-main>
      <!-- TODO: fix screen flickering between private and unprivate pages -->
      <router-view />
    </v-main>
  </v-app>
</template>
