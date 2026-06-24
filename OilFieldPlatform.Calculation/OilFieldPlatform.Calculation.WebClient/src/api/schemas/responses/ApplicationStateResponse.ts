export const type = 'application.state'

export interface ProjectModel {
  id: number
  name: string
  isChanged: boolean
}

export interface ApplicationState {
  project: ProjectModel | null
}

export interface ApplicationStateResponse {
  type: 'application.state'
  state: ApplicationState
}
