<script setup lang="ts">
import { useMutation, useQuery } from "@tanstack/vue-query"
import axios from "axios"

import { useAuth } from "@/utils/composables"

interface Lock {
  lockId: number
  lockName: string
  macAddress: string
}

type GetMyLocksResponse = Array<Lock>

const { user } = useAuth()

const {
  data: myLocks,
  isLoading: isLoadingLocks,
  refetch: refetchMyLocks,
} = useQuery(["my-locks"], () => {
  return axios
    .get<GetMyLocksResponse>(`/api/locks/users/${user.value!.userId}`)
    .then((res) => res.data)
})

const { mutateAsync: unpairLock, isLoading: isUnpairingALock } = useMutation(
  (lockId: number) => {
    return axios.delete(
      `/api/locks/unpair?lockId=${lockId}&adminId=${user.value!.userId}`,
    )
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
              @click="unpairLock(lock.lockId).then(() => refetchMyLocks)"
              :disabled="isUnpairingALock"
            >
              <v-icon>mdi-delete</v-icon>
            </v-btn>
          </v-list-item-action>
          <v-list-item-action>
            <v-btn icon size="small">
              <v-icon>mdi-pencil</v-icon>
            </v-btn>
          </v-list-item-action>
        </template>
        <v-list-item-title>{{ lock.lockName }}</v-list-item-title>
        <v-list-item-subtitle>{{ lock.macAddress }}</v-list-item-subtitle>
      </v-list-item>
    </v-list>
  </v-card-text>
</template>
