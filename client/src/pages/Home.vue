<script setup lang="ts">
import { useAuth } from "@/utils/composables"

const { logout } = useAuth()
</script>

<template>
  <h1>Home</h1>
  <v-btn @click="logout"> logout </v-btn>
</template>
