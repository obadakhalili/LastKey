function callApi(method: "GET" | "POST" | "PUT" | "DELETE") {
  return async <ResponseBody = unknown>(
    route: string,
    data?: FormData | Record<string, unknown>,
  ) => {
    const response = await fetch(`/api/${route}`, {
      method,
      body: data instanceof FormData ? data : JSON.stringify(data),
    })

    if (!response.ok) {
      throw new Error(response.statusText)
    }

    return (await response.json()) as ResponseBody
  }
}

const fetcher = {
  post: callApi("POST"),
  get: callApi("GET"),
  put: callApi("PUT"),
  delete: callApi("DELETE"),
}

export default fetcher
