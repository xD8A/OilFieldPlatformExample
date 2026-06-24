export const type = 'application.log'

export interface LogResponse {
  type: 'application.log'
  level: string
  message: string
  exception?: string
}
