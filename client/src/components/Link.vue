<script setup lang="ts">
defineProps<{
  link: string
  text: string
}>()
</script>

<template>
  <router-link :to="link" class="no-underline">
    <v-btn variant="text" color="info" class="!lowercase">
      {{ text }}
    </v-btn>
  </router-link>
</template>
