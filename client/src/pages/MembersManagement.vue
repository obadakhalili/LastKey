<script setup lang="ts">
import { ref } from "vue"
import { useMutation, useQuery } from "@tanstack/vue-query"
import axios, { AxiosError } from "axios"

import fieldValidators from "@/utils/field-validators"

interface Member {
  userId: number
  fullName: string
  username: string
}

type GetMembersResponse = Member[]

const {
  data: members,
  isLoading: isLoadingMembers,
  refetch: refetchMembers,
} = useQuery(["members"], () => {
  return axios
    .get<GetMembersResponse>("/api/users/members")
    .then((res) => res.data)
})

const { mutateAsync: deleteMember, isLoading: isDeletingMember } = useMutation(
  (memberId: number) => {
    return axios.delete(`/api/users/members/${memberId}`)
  },
)

const isAddingANewMemberDialogOpen = ref(false)

interface AddMemberRequest {
  fullName: string
  userImage: File
  username: string
  password: string
}

const {
  mutateAsync: addMember,
  isLoading: isAddingAMember,
  failureReason: addMemberFailureReason,
} = useMutation<unknown, AxiosError<{ message: string }>, AddMemberRequest>(
  (member) => {
    const formData = new FormData()

    formData.append("fullName", member.fullName)
    formData.append("userImage", member.userImage)
    formData.append("username", member.username)
    formData.append("password", member.password)

    return axios.post("/api/users/members", formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    })
  },
)

type AddMemberFormInfo = Omit<AddMemberRequest, "userImage"> & {
  userImage: Array<File> | undefined
}

const newMemberInfo = ref<AddMemberFormInfo>({
  fullName: "",
  userImage: undefined,
  username: "",
  password: "",
})

const newMemberInfoIsValid = ref(false)

function handleAddMemberFormSubmit() {
  if (newMemberInfoIsValid.value) {
    addMember({
      fullName: newMemberInfo.value.fullName,
      userImage: newMemberInfo.value.userImage![0],
      username: newMemberInfo.value.username,
      password: newMemberInfo.value.password,
    })
      .then(() => refetchMembers())
      .then(() => {
        isAddingANewMemberDialogOpen.value = false
        newMemberInfo.value = {
          fullName: "",
          userImage: undefined,
          username: "",
          password: "",
        }
      })
  }
}
</script>

<template>
  <v-card-title>Members</v-card-title>
  <v-card-text>
    <v-progress-circular v-if="isLoadingMembers" indeterminate />
    <v-list v-else>
      <v-list-item v-for="member in members" :key="member.userId">
        <template v-slot:prepend>
          <v-list-item-action>
            <v-btn
              icon
              size="small"
              @click="deleteMember(member.userId).then(() => refetchMembers())"
              :disabled="isDeletingMember"
            >
              <v-icon>mdi-delete</v-icon>
            </v-btn>
          </v-list-item-action>
        </template>
        <v-list-item-title>{{ member.fullName }}</v-list-item-title>
        <v-list-item-subtitle>{{ member.username }}</v-list-item-subtitle>
      </v-list-item>
    </v-list>
  </v-card-text>
  <v-card-actions>
    <v-btn
      @click="isAddingANewMemberDialogOpen = true"
      prepend-icon="mdi-plus"
      color="primary"
      variant="flat"
    >
      Add
    </v-btn>
  </v-card-actions>
  <v-dialog v-model="isAddingANewMemberDialogOpen">
    <v-card>
      <v-card-title> Add a new member </v-card-title>
      <v-alert
        v-if="addMemberFailureReason?.response?.data.message"
        color="error"
        icon="mdi-alert-circle"
        class="mx-2"
        closable
      >
        {{ addMemberFailureReason?.response?.data.message }}
      </v-alert>
      <v-form
        v-model="newMemberInfoIsValid"
        @submit.prevent="handleAddMemberFormSubmit"
      >
        <v-card-text>
          <v-text-field
            label="Fullname"
            v-model="newMemberInfo.fullName"
            :rules="[fieldValidators.textRequired]"
          />
          <v-file-input
            clearable
            label="Verification image"
            accept="image/*"
            prepend-icon="mdi-camera"
            v-model="newMemberInfo.userImage"
            :rules="[fieldValidators.fileRequired]"
          ></v-file-input>
          <v-text-field
            label="Username"
            v-model="newMemberInfo.username"
            :rules="[fieldValidators.textRequired]"
          />
          <v-text-field
            type="password"
            label="Password"
            v-model="newMemberInfo.password"
            :rules="[fieldValidators.textRequired]"
          />
        </v-card-text>
        <v-card-actions>
          <v-btn
            type="submit"
            :loading="isAddingAMember || isLoadingMembers"
            prepend-icon="mdi-account-plus"
            color="primary"
          >
            Add member
          </v-btn>
        </v-card-actions>
      </v-form>
    </v-card>
  </v-dialog>
</template>
