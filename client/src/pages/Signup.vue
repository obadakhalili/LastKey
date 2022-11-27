<script setup lang="ts">
import { useMutation } from "@tanstack/vue-query"
import { reactive, ref } from "vue"
import axios from "axios"

import fieldValidators from "@/utils/field-validators"

interface SignupFormInfo {
  fullName: string
  userImage: Array<File> | undefined
  userName: string
  password: string
}

const signupFormInfo = reactive<SignupFormInfo>({
  fullName: "",
  userImage: undefined,
  userName: "",
  password: "",
})

const sginFormInfoIsValid = ref(false)

const {
  mutateAsync: registerUser,
  isLoading: isAddingUser,
  isSuccess: isUserAdded,
  isError: addingUserFailed,
} = useMutation({
  mutationFn: (signupInfo: SignupFormInfo) => {
    const formData = new FormData()
    formData.append("fullName", signupInfo.fullName)
    const [userImage] = signupInfo.userImage!
    formData.append("userImage", userImage, userImage.name)
    formData.append("userName", signupInfo.userName)
    formData.append("password", signupInfo.password)

    return axios.post("/api/users", formData)
  },
})

function handleRegisterFormSubmit() {
  if (sginFormInfoIsValid.value) {
    registerUser(signupFormInfo)
  }
}
</script>

<template>
  <v-form
    v-model="sginFormInfoIsValid"
    class="max-w-[700px] p-5 mx-auto space-y-3"
    @submit.prevent="handleRegisterFormSubmit"
  >
    <h1 class="text-h4 text-center">Register an Account</h1>
    <p
      class="text-text-subtitle-1"
      :class="{
        'opacity-0': !(addingUserFailed || isUserAdded),
        'opacity-100': addingUserFailed || isUserAdded,
        'text-red-500': addingUserFailed,
        'text-green-500': isUserAdded,
      }"
    >
      {{
        addingUserFailed ? "Something went wrong" : "User added successfully"
      }}
    </p>
    <v-text-field
      label="Fullname"
      v-model="signupFormInfo.fullName"
      :rules="[fieldValidators.textRequired]"
    ></v-text-field>
    <v-file-input
      clearable
      label="Verification image"
      accept="image/*"
      v-model="signupFormInfo.userImage"
      :rules="[fieldValidators.fileRequired]"
    ></v-file-input>
    <v-text-field
      label="Username"
      v-model="signupFormInfo.userName"
      :rules="[fieldValidators.textRequired]"
    ></v-text-field>
    <v-text-field
      type="password"
      label="Password"
      v-model="signupFormInfo.password"
      :rules="[fieldValidators.textRequired]"
    ></v-text-field>
    <v-btn
      type="submit"
      :disabled="isAddingUser"
      :prepend-icon="isAddingUser ? '' : 'mdi-account-plus'"
    >
      Sign Up
    </v-btn>
  </v-form>
</template>
