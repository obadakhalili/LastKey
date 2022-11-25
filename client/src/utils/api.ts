function callApi(method: "GET" | "POST" | "PUT" | "DELETE") {
  return async <ResponseBody = unknown>(route: string, data?: unknown) => {
    const response = await fetch(`/api/${route}`, {
      method,
      body: JSON.stringify(data),
      headers: {
        "Content-Type": "application/json",
      },
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
