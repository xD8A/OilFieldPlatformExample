export const type = 'application.openProject'

export interface ApplicationOpenProjectRequest {
  type: 'application.openProject'
  id: number
}

export function create(id: number): ApplicationOpenProjectRequest {
  return { type, id }
}
