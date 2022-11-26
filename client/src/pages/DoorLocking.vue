<script setup lang="ts">
import { useAuth } from "@/utils/composables"

const { logout } = useAuth()
</script>

<template>
  <v-btn @click="logout"> logout </v-btn>
</template>
