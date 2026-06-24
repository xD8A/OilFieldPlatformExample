export const type = 'pages.waterSample.changed'

export interface WaterSampleChangedResponse {
  type: 'pages.waterSample.changed'
  key: string
  isOutdated: boolean
  properties: Record<string, number | null>
}
