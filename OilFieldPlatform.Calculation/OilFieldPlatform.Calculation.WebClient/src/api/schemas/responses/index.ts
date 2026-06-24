import { type as _list, type CalcProjectProjection, type ApplicationListProjectsResponse } from './ApplicationListProjectsResponse.js'
import { type as _state, type ApplicationState, type ProjectModel, type ApplicationStateResponse } from './ApplicationStateResponse.js'
import { type as _opened, type ApplicationProjectOpenedResponse } from './ApplicationProjectOpenedResponse.js'
import { type as _saved, type ApplicationProjectSavedResponse } from './ApplicationProjectSavedResponse.js'
import { type as _closed, type ApplicationProjectClosedResponse } from './ApplicationProjectClosedResponse.js'
import { type as _changed, type ApplicationIsChangedResponse } from './ApplicationIsChangedResponse.js'
import { type as _wsState, type WaterSamplePageState, type WaterSampleProxy, type WaterSampleStateResponse } from './WaterSampleStateResponse.js'
import { type as _wsConnected, type WaterSampleConnectedResponse } from './WaterSampleConnectedResponse.js'
import { type as _wsDisconnected, type WaterSampleDisconnectedResponse } from './WaterSampleDisconnectedResponse.js'
import { type as _wsChanged, type WaterSampleChangedResponse } from './WaterSampleChangedResponse.js'
import { type as _wsCalculated, type WaterSampleCalculatedResponse } from './WaterSampleCalculatedResponse.js'
import { type as _wsAutoCalc, type WaterSampleAutoCalcSetResponse } from './WaterSampleAutoCalcSetResponse.js'
import { type as _log, type LogResponse } from './LogResponse.js'

export const ApplicationListProjectsResponse_type = _list
export const ApplicationStateResponse_type = _state
export const ApplicationProjectOpenedResponse_type = _opened
export const ApplicationProjectSavedResponse_type = _saved
export const ApplicationProjectClosedResponse_type = _closed
export const ApplicationIsChangedResponse_type = _changed
export const WaterSampleStateResponse_type = _wsState
export const WaterSampleConnectedResponse_type = _wsConnected
export const WaterSampleDisconnectedResponse_type = _wsDisconnected
export const WaterSampleChangedResponse_type = _wsChanged
export const WaterSampleCalculatedResponse_type = _wsCalculated
export const WaterSampleAutoCalcSetResponse_type = _wsAutoCalc
export const LogResponse_type = _log

export type { CalcProjectProjection, ApplicationListProjectsResponse } from './ApplicationListProjectsResponse.js'
export type { ApplicationState, ProjectModel, ApplicationStateResponse } from './ApplicationStateResponse.js'
export type { ApplicationProjectOpenedResponse } from './ApplicationProjectOpenedResponse.js'
export type { ApplicationProjectSavedResponse } from './ApplicationProjectSavedResponse.js'
export type { ApplicationProjectClosedResponse } from './ApplicationProjectClosedResponse.js'
export type { ApplicationIsChangedResponse } from './ApplicationIsChangedResponse.js'
export type { WaterSamplePageState, WaterSampleProxy, WaterSampleStateResponse } from './WaterSampleStateResponse.js'
export type { WaterSampleConnectedResponse } from './WaterSampleConnectedResponse.js'
export type { WaterSampleDisconnectedResponse } from './WaterSampleDisconnectedResponse.js'
export type { WaterSampleChangedResponse } from './WaterSampleChangedResponse.js'
export type { WaterSampleCalculatedResponse } from './WaterSampleCalculatedResponse.js'
export type { WaterSampleAutoCalcSetResponse } from './WaterSampleAutoCalcSetResponse.js'
export type { LogResponse } from './LogResponse.js'

export type IWebSocketResponse =
  | ApplicationListProjectsResponse
  | ApplicationStateResponse
  | ApplicationProjectOpenedResponse
  | ApplicationProjectSavedResponse
  | ApplicationProjectClosedResponse
  | ApplicationIsChangedResponse
  | WaterSampleStateResponse
  | WaterSampleConnectedResponse
  | WaterSampleDisconnectedResponse
  | WaterSampleChangedResponse
  | WaterSampleCalculatedResponse
  | WaterSampleAutoCalcSetResponse
  | LogResponse
