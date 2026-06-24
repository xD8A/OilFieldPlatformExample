export const type = 'pages.waterSample.edit'

export interface WaterSampleEditRequest {
  type: 'pages.waterSample.edit'
  key: string
  property: string
  value: number | null
}

export function create(key: string, property: string, value: number | null): WaterSampleEditRequest {
  return { type, key, property, value }
}
