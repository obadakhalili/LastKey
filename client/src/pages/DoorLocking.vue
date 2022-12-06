<script setup lang="ts">
import { ref } from "vue"
import { WifiWizard2 } from "@ionic-native/wifi-wizard-2"

import { useAuth } from "@/utils/composables"

const networks = ref<Array<any>>([])
const error = ref<string>("")

WifiWizard2.requestPermission().then(() => {
  WifiWizard2.scan().then((ntwrks) => {
    networks.value = ntwrks
  }).catch((err) => {
    error.value = err
  })
}).catch((err) => {
  error.value = err
})

const { logout } = useAuth()
</script>

<template>
  <v-btn @click="logout"> logout </v-btn>

  <!-- found networks count -->
  <v-text>Found {{ networks.length }} networks</v-text>

  <v-list>
    <v-list-item v-for="network in networks" :key="network.SSID">
      <v-list-item-title>{{ network.SSID }}</v-list-item-title>
      <v-list-item-title>{{ network.BSSID }}</v-list-item-title>
    </v-list-item>
  </v-list>

  <v-alert v-if="error" type="error">{{ error }}</v-alert>
</template>
