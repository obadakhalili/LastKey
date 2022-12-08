import { computed, ref } from "vue"
import { useMutation } from "@tanstack/vue-query"
import axios, { AxiosError, AxiosResponse } from "axios"

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
  const user = ref<UserInfoResponse | null | undefined>()

  axios
    .get("/api/users/me")
    .then((response: AxiosResponse<UserInfoResponse>) => {
      user.value = response.data
    })
    .catch(() => {
      user.value = null
    })

  return function useAuth() {
    const {
      mutateAsync: login,
      isLoading: isLoggingIn,
      failureReason,
      isError: isLoginError,
    } = useMutation<AxiosResponse<UserInfoResponse>, AxiosError, LoginRequest>({
      mutationKey: ["login"],
      mutationFn: (loginInfo) => {
        return axios.post("/api/users/login", loginInfo)
      },
      onSuccess: (response) => {
        user.value = response.data
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

    return {
      user,
      login,
      loginAttemptUnauthorized,
      isLoginError,
      isLoggingIn,
      logout,
    }
  }
}

export const useAuth = createAuth()
