import { useQuery, UseQueryOptions } from "@tanstack/vue-query"
import axios from "axios"

export interface Lock {
  lockId: number
  macAddress: string
  lockName: string
  isLocked: boolean
}

type GetMyLocksResponse = Array<Lock>

export function useMyLocks(
  options?: Omit<UseQueryOptions<GetMyLocksResponse>, "queryKey" | "queryFn">,
) {
  return useQuery(
    ["my-locks"],
    () => {
      return axios.get<GetMyLocksResponse>("/api/locks").then((res) => res.data)
    },
    options,
  )
}
