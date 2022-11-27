import { ref } from "vue"
import { useMutation } from "@tanstack/vue-query"
import axios from "axios"
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
      isError: isLoginError,
    } = useMutation({
      mutationKey: ["login"],
      mutationFn: (loginInfo: LoginRequest) => {
        return axios.post<unknown, UserInfoResponse>(
          "/api/users/login",
          loginInfo,
        )
      },
      onSuccess: (data) => {
        user.value = data
      },
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
        const userInfo = await axios.get<unknown, UserInfoResponse>(
          `/api/users/user/${decodedPayload.userId}`,
        )

        user.value = userInfo
      } catch {
        user.value = null
      }
    }

    return {
      user,
      login,
      isLoggingIn,
      isLoginError,
      logout,
      verifyTokenPayloadCookie,
    }
  }
}

export const useAuth = createAuth()
