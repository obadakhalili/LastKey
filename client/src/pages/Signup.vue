<script setup lang="ts">
import { reactive, ref } from "vue"
import { useMutation } from "@tanstack/vue-query"
import axios from "axios"

import fieldValidators from "@/utils/field-validators"
import { useAuth } from "@/utils/composables"
import Link from "@/components/Link.vue"

interface SignupRequest {
  fullName: string
  userImage: File
  username: string
  password: string
}

type SignupFormInfo = Omit<SignupRequest, "userImage"> & {
  userImage: Array<File> | undefined
}

const signupFormInfo = reactive<SignupFormInfo>({
  fullName: "",
  userImage: undefined,
  username: "",
  password: "",
})

const sginFormInfoIsValid = ref(false)

const {
  mutateAsync: registerUser,
  isLoading: isAddingUser,
  isSuccess: isUserAdded,
  isError: isLoginError,
} = useMutation({
  mutationFn: (signupInfo: SignupRequest) => {
    const formData = new FormData()
    formData.append("fullName", signupInfo.fullName)
    formData.append("userImage", signupInfo.userImage, signupInfo.userImage.name)
    formData.append("userName", signupInfo.username)
    formData.append("password", signupInfo.password)

    return axios.post("/api/users", formData)
  },
})

const { login, isLoggingIn } = useAuth()

function handleRegisterFormSubmit() {
  if (sginFormInfoIsValid.value) {
    registerUser(
      {
        ...signupFormInfo,
        userImage: signupFormInfo.userImage![0],
      },
      {
        onSuccess: () => {
          login({
            username: signupFormInfo.username,
            password: signupFormInfo.password,
          })
        },
      },
    )
  }
}
</script>

<template>
  <v-form
    v-model="sginFormInfoIsValid"
    class="max-w-[700px] p-5 mx-auto space-y-3"
    @submit.prevent="handleRegisterFormSubmit"
  >
    <h1 class="text-h4 text-center">Register an account</h1>
    <p
      class="text-subtitle-1"
      :class="{
        'opacity-0': !(isLoginError || isUserAdded),
        'opacity-100': isLoginError || isUserAdded,
        'text-red-500': isLoginError,
        'text-green-500': isUserAdded,
      }"
    >
      {{ isLoginError ? "Something went wrong" : "User added successfully" }}
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
      prepend-icon="mdi-camera"
      v-model="signupFormInfo.userImage"
      :rules="[fieldValidators.fileRequired]"
    ></v-file-input>
    <v-text-field
      label="Username"
      v-model="signupFormInfo.username"
      :rules="[fieldValidators.textRequired]"
    ></v-text-field>
    <v-text-field
      type="password"
      label="Password"
      v-model="signupFormInfo.password"
      :rules="[fieldValidators.textRequired]"
    ></v-text-field>
    <div class="flex justify-between">
      <Link link="/login" text="Login" />
      <v-btn
        type="submit"
        :disabled="isAddingUser || isLoggingIn"
        :prepend-icon="
          isAddingUser || isLoggingIn
            ? 'mdi-loading mdi-spin'
            : 'mdi-account-plus'
        "
        :color="isLoggingIn ? 'success' : ''"
      >
        {{
          isLoggingIn
            ? "Logging in..."
            : isAddingUser
            ? "Registering..."
            : "Register"
        }}
      </v-btn>
    </div>
  </v-form>
</template>
