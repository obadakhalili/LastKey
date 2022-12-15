<script setup lang="ts">
import { computed, ref } from "vue"
import { useMutation } from "@tanstack/vue-query"
import axios from "axios"
import { Capacitor } from "@capacitor/core"
import { WifiWizard2 } from "@ionic-native/wifi-wizard-2"

import fieldValidators from "@/utils/field-validators"
import { useMyLocks } from "@/utils/apis"

const {
  data: myLocks,
  isLoading: isLoadingLocks,
  refetch: refetchMyLocks,
} = useMyLocks()

const { mutateAsync: unpairLock, isLoading: isUnpairingALock } = useMutation(
  (lockId: number) => {
    return axios.delete(`/api/locks/${lockId}`)
  },
)

const { mutateAsync: updateLockName, isLoading: isUpadingALockName } =
  useMutation((lockId: number) => {
    return axios.patch(`/api/locks/${lockId}/name/${newLockName.value}`)
  })

const lockToEdit = ref<number | null>(null)
const newLockName = ref("")
const isEditingALockDialogOpen = computed(() => lockToEdit.value !== null)

const isPairingANewLockDialogOpen = ref(false)

const accessPoints = ref<
  | Array<{
      SSID: string
      BSSID: string
    }>
  | null
  | undefined
>()

// NOTE: why chaining didn't work here?
WifiWizard2.requestPermission()
  .then(() => {
    WifiWizard2.scan()
      .then((aps) => {
        accessPoints.value = aps
      })
      .catch(() => {
        accessPoints.value = null
      })
  })
  .catch(() => {
    accessPoints.value = null
  })

const { mutateAsync: pairToALock, isLoading: isPairingToALock } = useMutation(
  (lock: { name: string; macAddress: string }) => {
    return axios.post("/api/locks", {
      lockMacAddress: lock.macAddress,
      // NOTE: this is unreliable as the name could already be taken
      lockName: lock.name,
    })
  },
)
</script>

<template>
  <v-card-title>My Locks</v-card-title>
  <v-card-text>
    <v-progress-circular v-if="isLoadingLocks" indeterminate />
    <v-list v-else>
      <v-list-item v-for="lock in myLocks" :key="lock.lockId">
        <template v-slot:prepend>
          <v-list-item-action>
            <v-btn
              icon
              size="small"
              @click="unpairLock(lock.lockId).then(() => refetchMyLocks())"
              :disabled="isUnpairingALock"
            >
              <v-icon>mdi-delete</v-icon>
            </v-btn>
          </v-list-item-action>
          <v-list-item-action>
            <v-btn icon size="small" @click="lockToEdit = lock.lockId">
              <v-icon>mdi-pencil</v-icon>
            </v-btn>
          </v-list-item-action>
        </template>
        <v-list-item-title>{{ lock.lockName }}</v-list-item-title>
        <v-list-item-subtitle>{{ lock.macAddress }}</v-list-item-subtitle>
      </v-list-item>
    </v-list>
    <v-card-actions>
      <v-btn
        @click="isPairingANewLockDialogOpen = true"
        prepend-icon="mdi-plus"
        color="primary"
        variant="flat"
        :disabled="!Capacitor.isNativePlatform()"
      >
        Add
      </v-btn>
    </v-card-actions>
  </v-card-text>
  <v-dialog :persistent="true" v-model="isEditingALockDialogOpen" max-width="290">
    <v-card>
      <v-card-title> Update lock name </v-card-title>
      <v-form
        @submit.prevent="
          updateLockName(lockToEdit!).then(() => {
            lockToEdit = null
            newLockName = ''
            refetchMyLocks()
          })
        "
        validate-on="submit"
      >
        <v-card-text>
          <v-text-field
            label="New lock name"
            v-model="newLockName"
            :rules="[fieldValidators.textRequired]"
          />
        </v-card-text>
        <v-card-actions>
          <v-btn @click=";(newLockName = ''), (lockToEdit = null)">
            Close
          </v-btn>
          <v-btn type="submit" color="primary" :loading="isUpadingALockName">
            Save
          </v-btn>
        </v-card-actions>
      </v-form>
    </v-card>
  </v-dialog>
  <v-dialog
    v-model="isPairingANewLockDialogOpen"
    fullscreen
    transition="dialog-bottom-transition"
  >
    <v-card>
      <v-toolbar dark color="primary">
        <v-btn icon dark @click="isPairingANewLockDialogOpen = false">
          <v-icon>mdi-close</v-icon>
        </v-btn>
        <v-toolbar-title> Pair to a new lock </v-toolbar-title>
      </v-toolbar>
      <v-card-text>
        <v-progress-circular v-if="accessPoints === undefined" indeterminate />
        <v-alert v-else-if="accessPoints === null" type="error">
          Failed to scan for access points
        </v-alert>
        <v-list v-else>
          <v-list-item
            v-for="(accessPoint, index) in accessPoints"
            :key="index"
          >
            <template v-slot:prepend>
              <v-list-item-action>
                <v-btn
                  icon
                  size="small"
                  @click="
                    pairToALock({
                      name: accessPoint.SSID,
                      macAddress: accessPoint.BSSID,
                    })
                      .then(() => refetchMyLocks())
                      .then(() => (isPairingANewLockDialogOpen = false))
                  "
                  :disabled="
                    isPairingToALock ||
                    myLocks?.some(
                      (lock) => lock.macAddress === accessPoint.BSSID,
                    )
                  "
                >
                  <v-icon>mdi-link</v-icon>
                </v-btn>
              </v-list-item-action>
            </template>
            <v-list-item-title>{{ accessPoint.SSID }}</v-list-item-title>
            <v-list-item-subtitle>
              {{ accessPoint.BSSID }}
            </v-list-item-subtitle>
          </v-list-item>
        </v-list>
      </v-card-text>
    </v-card>
  </v-dialog>
</template>
