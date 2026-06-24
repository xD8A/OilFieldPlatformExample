export const type = 'application.projects'

export interface CalcProjectProjection {
  id: number
  name: string
}

export interface ApplicationListProjectsResponse {
  type: 'application.projects'
  projects: CalcProjectProjection[]
}
