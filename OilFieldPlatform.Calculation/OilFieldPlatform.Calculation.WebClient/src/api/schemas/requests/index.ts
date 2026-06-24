import { type as _list } from './ApplicationListProjectRequest.js'
import { type as _getState } from './ApplicationGetStateRequest.js'
import { type as _openProject, create as _createOpenProject } from './ApplicationOpenProjectRequest.js'
import { type as _save } from './ApplicationSaveProjectRequest.js'
import { type as _close } from './ApplicationCloseProjectRequest.js'
import { type as _wsGetState } from './WaterSampleGetStateRequest.js'
import { type as _wsConnect } from './WaterSampleConnectRequest.js'
import { type as _wsDisconnect } from './WaterSampleDisconnectRequest.js'
import { type as _wsEdit, create as _createEdit } from './WaterSampleEditRequest.js'
import { type as _wsAutoCalc, create as _createAutoCalc } from './WaterSampleSetAutoCalcRequest.js'
import { type as _wsCalc } from './WaterSampleCalculateRequest.js'

export const ApplicationListProjectRequest_type = _list
export const ApplicationGetStateRequest_type = _getState
export const ApplicationOpenProjectRequest_type = _openProject
export const ApplicationSaveProjectRequest_type = _save
export const ApplicationCloseProjectRequest_type = _close
export const WaterSampleGetStateRequest_type = _wsGetState
export const WaterSampleConnectRequest_type = _wsConnect
export const WaterSampleDisconnectRequest_type = _wsDisconnect
export const WaterSampleEditRequest_type = _wsEdit
export const WaterSampleSetAutoCalcRequest_type = _wsAutoCalc
export const WaterSampleCalculateRequest_type = _wsCalc

export { create as createOpenProjectRequest } from './ApplicationOpenProjectRequest.js'
export { create as createEditRequest } from './WaterSampleEditRequest.js'
export { create as createAutoCalcRequest } from './WaterSampleSetAutoCalcRequest.js'

export type { ApplicationListProjectRequest } from './ApplicationListProjectRequest.js'
export type { ApplicationGetStateRequest } from './ApplicationGetStateRequest.js'
export type { ApplicationOpenProjectRequest } from './ApplicationOpenProjectRequest.js'
export type { ApplicationSaveProjectRequest } from './ApplicationSaveProjectRequest.js'
export type { ApplicationCloseProjectRequest } from './ApplicationCloseProjectRequest.js'
export type { WaterSampleGetStateRequest } from './WaterSampleGetStateRequest.js'
export type { WaterSampleConnectRequest } from './WaterSampleConnectRequest.js'
export type { WaterSampleDisconnectRequest } from './WaterSampleDisconnectRequest.js'
export type { WaterSampleEditRequest } from './WaterSampleEditRequest.js'
export type { WaterSampleSetAutoCalcRequest } from './WaterSampleSetAutoCalcRequest.js'
export type { WaterSampleCalculateRequest } from './WaterSampleCalculateRequest.js'

export type IWebSocketRequest =
  | ApplicationListProjectRequest
  | ApplicationGetStateRequest
  | ApplicationOpenProjectRequest
  | ApplicationSaveProjectRequest
  | ApplicationCloseProjectRequest
  | WaterSampleGetStateRequest
  | WaterSampleConnectRequest
  | WaterSampleDisconnectRequest
  | WaterSampleEditRequest
  | WaterSampleSetAutoCalcRequest
  | WaterSampleCalculateRequest
