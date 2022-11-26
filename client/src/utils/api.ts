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

    try {
      return (await response.json()) as ResponseBody
    } catch {}
  }
}

const fetcher = {
  post: callApi("POST"),
  get: callApi("GET"),
  put: callApi("PUT"),
  delete: callApi("DELETE"),
}

export default fetcher
