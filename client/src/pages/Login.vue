<script setup lang="ts">
import { reactive, ref } from "vue"

import fieldValidators from "@/utils/field-validators"
import { useAuth, LoginRequest } from "@/utils/composables"
import Link from "@/components/Link.vue"

const loginFormInfo = reactive<LoginRequest>({
  username: "",
  password: "",
})

const loginFormInfoIsValid = ref(false)

const { login, loginAttemptUnauthorized, isLoggingIn, isLoginError } = useAuth()

function handleLoginFormSubmit() {
  if (loginFormInfoIsValid.value) {
    login(loginFormInfo)
  }
}
</script>

<template>
  <v-form
    v-model="loginFormInfoIsValid"
    class="max-w-[700px] p-5 mx-auto space-y-3"
    @submit.prevent="handleLoginFormSubmit"
  >
    <h1 class="text-h4 text-center">Login to your Account</h1>
    <v-alert v-if="isLoginError" color="error" icon="mdi-alert-circle" closable>
      {{
        loginAttemptUnauthorized
          ? "Username or password is incorrect"
          : "An error occurred while logging in"
      }}
    </v-alert>
    <v-text-field
      label="Username"
      v-model="loginFormInfo.username"
      :rules="[fieldValidators.textRequired]"
    ></v-text-field>
    <v-text-field
      type="password"
      label="Password"
      v-model="loginFormInfo.password"
      :rules="[fieldValidators.textRequired]"
    ></v-text-field>
    <div class="flex justify-between">
      <Link link="/signup" text="Create an account" />
      <v-btn
        type="submit"
        :disabled="isLoggingIn"
        color="success"
        :prepend-icon="isLoggingIn ? 'mdi-loading mdi-spin' : 'mdi-login'"
      >
        {{ isLoggingIn ? "Logging in..." : "Login" }}
      </v-btn>
    </div>
  </v-form>
</template>
