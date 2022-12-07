import { computed, ref } from "vue"
import { useMutation } from "@tanstack/vue-query"
import axios, { AxiosError, AxiosResponse } from "axios"
import jwtDecode from "jwt-decode"

import { findCookie } from "./helpers"

export interface LoginRequest {
  username: string
  password: string
}

export interface UserInfoResponse {
  userId: number
  fullName: string
  username: string
  isAdmin: boolean
}

function createAuth() {
  const user = ref<UserInfoResponse | null | undefined>(undefined)

  return function useAuth() {
    const {
      mutateAsync: login,
      isLoading: isLoggingIn,
      failureReason,
      isError: isLoginError,
    } = useMutation<UserInfoResponse, AxiosError, LoginRequest>({
      mutationKey: ["login"],
      mutationFn: (loginInfo) => {
        return axios.post("/api/users/login", loginInfo)
      },
      onSuccess: (data) => {
        user.value = data
      },
    })

    const loginAttemptUnauthorized = computed(() => {
      return failureReason.value?.response?.status === 401
    })

    const { mutateAsync: logout } = useMutation({
      mutationKey: ["logout"],
      mutationFn: () => {
        return axios.post("/api/users/logout")
      },
      onSuccess: () => {
        user.value = null
      },
    })

    async function verifyTokenPayloadCookie() {
      try {
        const encodedPayload = findCookie("jwtPayload")
        const jwtToken = `header.${encodedPayload}.signature`
        const decodedPayload = jwtDecode<{ userId: number | undefined }>(
          jwtToken,
        )
        const response = await axios.get<
          unknown,
          AxiosResponse<UserInfoResponse>
        >(`/api/users/user/${decodedPayload.userId}`)

        user.value = response.data
      } catch {
        user.value = null
      }
    }

    return {
      user,
      login,
      loginAttemptUnauthorized,
      isLoginError,
      isLoggingIn,
      logout,
      verifyTokenPayloadCookie,
    }
  }
}

export const useAuth = createAuth()
