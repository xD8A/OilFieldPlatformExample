import { useWebSocket } from '../composables/useWebSocket.js'
import {
  ApplicationListProjectRequest_type, ApplicationGetStateRequest_type,
  ApplicationOpenProjectRequest_type, ApplicationSaveProjectRequest_type,
  ApplicationCloseProjectRequest_type, WaterSampleGetStateRequest_type,
  WaterSampleConnectRequest_type, WaterSampleDisconnectRequest_type,
  WaterSampleEditRequest_type, WaterSampleSetAutoCalcRequest_type,
  WaterSampleCalculateRequest_type,
} from './schemas/requests/index.js'
import {
  ApplicationListProjectsResponse_type, ApplicationStateResponse_type,
  ApplicationProjectOpenedResponse_type, ApplicationProjectSavedResponse_type,
  ApplicationProjectClosedResponse_type, ApplicationIsChangedResponse_type,
  WaterSampleStateResponse_type, WaterSampleConnectedResponse_type,
  WaterSampleDisconnectedResponse_type, WaterSampleChangedResponse_type,
  WaterSampleCalculatedResponse_type, WaterSampleAutoCalcSetResponse_type,
  LogResponse_type,
} from './schemas/responses/index.js'
import type {
  ApplicationListProjectsResponse, ApplicationStateResponse,
  ApplicationIsChangedResponse, WaterSampleStateResponse,
  WaterSampleChangedResponse, WaterSampleAutoCalcSetResponse,
  LogResponse,
} from './schemas/responses/index.js'

const ws = useWebSocket()

// ——— Connection ———

export const connected = ws.connected
export const connecting = ws.connecting

export function connect(): void {
  ws.connect()
}

export function disconnect(): void {
  ws.disconnect()
}

// ——— Requests ———

export function listProjects(): void {
  ws.send(ApplicationListProjectRequest_type)
}

export function getAppState(): void {
  ws.send(ApplicationGetStateRequest_type)
}

export function openProject(id: number): void {
  ws.send(ApplicationOpenProjectRequest_type, { id })
}

export function saveProject(): void {
  ws.send(ApplicationSaveProjectRequest_type)
}

export function closeProject(): void {
  ws.send(ApplicationCloseProjectRequest_type)
}

export function getWaterSampleState(): void {
  ws.send(WaterSampleGetStateRequest_type)
}

export function connectWaterSamples(): void {
  ws.send(WaterSampleConnectRequest_type)
}

export function disconnectWaterSamples(): void {
  ws.send(WaterSampleDisconnectRequest_type)
}

export function editSample(key: string, property: string, value: number | null): void {
  ws.send(WaterSampleEditRequest_type, { key, property, value })
}

export function setAutoCalc(isAuto: boolean): void {
  ws.send(WaterSampleSetAutoCalcRequest_type, { isAuto })
}

export function calculate(): void {
  ws.send(WaterSampleCalculateRequest_type)
}

// ——— Response subscriptions ———

export function onProjects(cb: (msg: ApplicationListProjectsResponse) => void): () => void {
  return ws.on(ApplicationListProjectsResponse_type, cb)
}

export function onAppState(cb: (msg: ApplicationStateResponse) => void): () => void {
  return ws.on(ApplicationStateResponse_type, cb)
}

export function onProjectOpened(cb: () => void): () => void {
  return ws.on(ApplicationProjectOpenedResponse_type, cb)
}

export function onProjectSaved(cb: () => void): () => void {
  return ws.on(ApplicationProjectSavedResponse_type, cb)
}

export function onProjectClosed(cb: () => void): () => void {
  return ws.on(ApplicationProjectClosedResponse_type, cb)
}

export function onIsChanged(cb: (msg: ApplicationIsChangedResponse) => void): () => void {
  return ws.on(ApplicationIsChangedResponse_type, cb)
}

export function onWaterSampleState(cb: (msg: WaterSampleStateResponse) => void): () => void {
  return ws.on(WaterSampleStateResponse_type, cb)
}

export function onWaterSampleConnected(cb: () => void): () => void {
  return ws.on(WaterSampleConnectedResponse_type, cb)
}

export function onWaterSampleChanged(cb: (msg: WaterSampleChangedResponse) => void): () => void {
  return ws.on(WaterSampleChangedResponse_type, cb)
}

export function onWaterSampleCalculated(cb: () => void): () => void {
  return ws.on(WaterSampleCalculatedResponse_type, cb)
}

export function onAutoCalcSet(cb: (msg: WaterSampleAutoCalcSetResponse) => void): () => void {
  return ws.on(WaterSampleAutoCalcSetResponse_type, cb)
}

export function onLog(cb: (msg: LogResponse) => void): () => void {
  return ws.on(LogResponse_type, cb)
}

export function onOpen(fn: () => void): () => void {
  return ws.onOpen(fn)
}
