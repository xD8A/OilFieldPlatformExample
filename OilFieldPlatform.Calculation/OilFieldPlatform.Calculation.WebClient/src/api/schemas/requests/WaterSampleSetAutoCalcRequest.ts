export const type = 'pages.waterSample.setAutoCalc'

export interface WaterSampleSetAutoCalcRequest {
  type: 'pages.waterSample.setAutoCalc'
  isAuto: boolean
}

export function create(isAuto: boolean): WaterSampleSetAutoCalcRequest {
  return { type, isAuto }
}
