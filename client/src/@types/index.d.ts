interface ImportMetaEnv {
  readonly VITE_BE_SERVER_URL: string | undefined
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}
