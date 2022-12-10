<script setup lang="ts">
import { ref } from "vue"
import { useMutation } from "@tanstack/vue-query"
import axios from "axios"
import { Camera, CameraResultType, CameraSource } from "@capacitor/camera"

import { useMyLocks, Lock } from "@/utils/apis"

const {
  data: myLocks,
  isLoading: isLoadingLocks,
  refetch: refetchMyLocks,
} = useMyLocks({
  onSuccess: (locks) => {
    if (locks.length > 0) {
      selectedLock.value = locks[0]
    }
  },
})
const selectedLock = ref<Lock | undefined>()

const {
  mutateAsync: changeLookState,
  isLoading: isChangingLockState,
  isError: isErrorInMutatingLockingState,
} = useMutation((vars: { lock: true } | { lock: false; image: string }) => {
  const lockId = selectedLock.value!.lockId

  if (!vars.lock) {
    return axios.patch<unknown, unknown, { image: string }>(
      `/api/locks/${lockId}/unlock`,
      { image: vars.image },
    )
  }

  return axios.patch(`/api/locks/${lockId}/lock`)
})

async function handleLockClick() {
  try {
    if (selectedLock.value!.isLocked) {
      const image = await Camera.getPhoto({
        quality: 90,
        allowEditing: false,
        resultType: CameraResultType.Base64,
        source: CameraSource.Camera,
      })

      return await changeLookState({
        lock: false,
        image: image.base64String!,
      })
    }

    await changeLookState({ lock: true })
  } catch {
  } finally {
    refetchMyLocks()
  }
}
</script>

<template>
  <v-card-title>Lock/Unlock Door</v-card-title>
  <v-alert
    v-if="isErrorInMutatingLockingState"
    closable
    text="Couldn't lock/unlock door"
    type="error"
  />
  <v-card-text>
    <v-select
      label="Select a look to (un)lock"
      :items="myLocks"
      item-title="lockName"
      item-value="lockId"
      v-model="selectedLock"
      :loading="isLoadingLocks"
    ></v-select>
    <v-row justify="center" class="my-15">
      <v-btn
        class="!h-20 !w-20"
        :loading="isChangingLockState || isLoadingLocks"
        :disabled="!selectedLock"
        size="x-large"
        :color="
          isErrorInMutatingLockingState
            ? 'error'
            : selectedLock
            ? selectedLock.isLocked
              ? 'info'
              : 'green'
            : 'grey'
        "
        :icon="
          selectedLock
            ? selectedLock.isLocked
              ? 'mdi-lock-open'
              : 'mdi-lock'
            : 'mdi-lock'
        "
        @click="handleLockClick"
    /></v-row>
  </v-card-text>
</template>
