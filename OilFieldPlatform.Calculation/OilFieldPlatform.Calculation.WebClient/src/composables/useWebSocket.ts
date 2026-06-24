import { ref, type Ref } from 'vue'
import * as Req from '../api/schemas/requests/index.js'
import * as Res from '../api/schemas/responses/index.js'

const SESSION_KEY = 'ws_session'

type Listener = (msg: any) => void
type OpenHandler = () => void

const listeners: Record<string, Listener[]> = {}
const openHandlers: OpenHandler[] = []
let ws: WebSocket | null = null
let reconnectTimer: ReturnType<typeof setTimeout> | null = null
let connectResolve: (() => void) | null = null

export const connected: Ref<boolean> = ref(false)
export const connecting: Ref<boolean> = ref(false)

export { Req, Res }

/** Сохранённый sessionId (из session.info). */
export function getSessionId(): string | null {
  return localStorage.getItem(SESSION_KEY)
}

export function useWebSocket() {
  function connect(): Promise<void> | undefined {
    if (ws && (ws.readyState === WebSocket.OPEN || ws.readyState === WebSocket.CONNECTING)) {
      return
    }
    connecting.value = true

    const promise = new Promise<void>((resolve) => {
      connectResolve = resolve
    })

    const protocol = location.protocol === 'https:' ? 'wss:' : 'ws:'
    const sid = localStorage.getItem(SESSION_KEY)
    const url = sid ? `${protocol}//${location.host}/ws?sessionId=${sid}` : `${protocol}//${location.host}/ws`
    ws = new WebSocket(url)

    ws.onopen = () => {
      connected.value = true
      connecting.value = false
      if (connectResolve) {
        connectResolve()
        connectResolve = null
      }
      openHandlers.forEach(fn => fn())
    }

    ws.onclose = () => {
      connected.value = false
      connecting.value = false
      ws = null
      scheduleReconnect()
    }

    ws.onerror = () => {
      connecting.value = false
    }

    ws.onmessage = (event: MessageEvent) => {
      try {
        const msg = JSON.parse(event.data as string)

        if (msg.type === 'session.info' && msg.sessionId) {
          localStorage.setItem(SESSION_KEY, msg.sessionId)
        }

        if (listeners[msg.type]) {
          listeners[msg.type].forEach(fn => fn(msg))
        }
        if (listeners['*']) {
          listeners['*'].forEach(fn => fn(msg))
        }
      } catch (e) {
        console.error('WS parse error:', e)
      }
    }

    return promise
  }

  function scheduleReconnect(): void {
    clearTimeout(reconnectTimer!)
    reconnectTimer = setTimeout(() => connect(), 3000)
  }

  function send(type: string, payload?: Record<string, unknown>): void {
    if (!ws || ws.readyState !== WebSocket.OPEN) {
      console.warn('WS not connected, cannot send:', type)
      return
    }
    ws.send(JSON.stringify(payload ? { type, ...payload } : { type }))
  }

  function on(type: string, fn: Listener): () => void {
    if (!listeners[type]) listeners[type] = []
    listeners[type].push(fn)
    return () => {
      const idx = (listeners[type] || []).indexOf(fn)
      if (idx !== -1) listeners[type].splice(idx, 1)
    }
  }

  function onOpen(fn: OpenHandler): () => void {
    openHandlers.push(fn)
    return () => {
      const idx = openHandlers.indexOf(fn)
      if (idx !== -1) openHandlers.splice(idx, 1)
    }
  }

  function disconnect(): void {
    clearTimeout(reconnectTimer!)
    if (ws) {
      ws.onclose = null
      ws.close()
      ws = null
    }
    connected.value = false
    connecting.value = false
  }

  return { connect, send, on, onOpen, disconnect, connected, connecting }
}
