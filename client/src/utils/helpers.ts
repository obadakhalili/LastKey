export function findCookie(name: string): string | undefined {
  const cookies = document.cookie.split(";").reduce((cookies, cookie) => {
    const [key, value] = cookie.split("=")
    return { ...cookies, [key.trim()]: value }
  }, {} as Record<string, string>)

  return cookies[name]
}
