<script setup lang="ts">
import { computed, watch, onMounted } from "vue"
import { RouterView, useRoute, useRouter } from "vue-router"

import { useAuth } from "./utils/composables"

const router = useRouter()
const route = useRoute()
const routeIsPrivate = computed(() => route.meta.private as boolean | undefined)
const { user, setupUser } = useAuth()

watch([user, routeIsPrivate], ([user, routeIsPrivate]) => {
  if (user === undefined || routeIsPrivate === undefined) {
    return
  }

  if (user && !routeIsPrivate) {
    router.push({ name: "Home" })
  } else if (!user && routeIsPrivate) {
    router.push({ name: "Login" })
  }
})

router.beforeEach((to, from, next) => {
  if (user.value === undefined) {
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
  setupUser()
})
</script>

<template>
  <v-app>
    <v-navigation-drawer expand-on-hover rail v-if="user">
      <v-list>
        <v-list-item
          :title="user.fullName"
          :subtitle="user.username"
        ></v-list-item>
      </v-list>
      <v-divider></v-divider>
      <v-list density="compact" nav>
        <v-list-item prepend-icon="mdi-home" title="Home" to="/" />
        <v-list-item
          v-if="user.isAdmin"
          prepend-icon="mdi-lock"
          title="Locks Management"
          to="/locks-management"
        />
      </v-list>
      <!-- TODO: add logout -->
    </v-navigation-drawer>
    <v-main>
      <v-row
        v-if="user === undefined"
        justify="center"
        align="center"
        class="h-screen"
      >
        <v-progress-circular
          color="primary"
          indeterminate
          size="64"
        ></v-progress-circular>
      </v-row>
      <div class="p-2">
        <router-view v-if="user !== undefined" />
      </div>
    </v-main>
  </v-app>
</template>
