export function textRequired(value: unknown): true | string {
  return !!value || "This field is required"
}

export function fileRequired(value: Array<File>): true | string {
  return value.length > 0 || "Please select a file"
}

const fieldValidators = {
  textRequired,
  fileRequired,
}

export default fieldValidators
