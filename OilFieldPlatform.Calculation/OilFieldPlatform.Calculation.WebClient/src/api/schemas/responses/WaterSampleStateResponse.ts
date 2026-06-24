export const type = 'pages.waterSample.state'

export interface WaterSampleProxy {
  key: string
  sampledAt: string
  waterType: number
  clusterStationName: string | null
  wellName: string | null
  isOutdated: boolean
  chloride: number | null
  carbonate: number | null
  bicarbonate: number | null
  sulfate: number | null
  calcium: number | null
  magnesium: number | null
  sodium: number | null
  chlorideEquivalent: number | null
  carbonateEquivalent: number | null
  bicarbonateEquivalent: number | null
  sulfateEquivalent: number | null
  calciumEquivalent: number | null
  magnesiumEquivalent: number | null
  sodiumEquivalent: number | null
}

export interface WaterSamplePageState {
  list: WaterSampleProxy[]
  isAutoCalc: boolean
}

export interface WaterSampleStateResponse {
  type: 'pages.waterSample.state'
  state: WaterSamplePageState
}
