<template>
  <div class="app">
    <AppHeader
      :project-name="projectName"
      :is-changed="isChanged"
      :connected="connected"
      @select="selectProject"
      @save="onSave"
      @close="onClose"
    />
    <ProjectDialog
      v-if="showProjectDialog"
      :projects="projects"
      @open="onOpenProject"
      @close="showProjectDialog = false"
    />
    <WaterSampleCalcPage
      :project-name="projectName"
      :is-auto-calc="isAutoCalc"
      :samples="samples"
      :connected="connected"
      @set-auto-calc="onSetAutoCalc"
      @calculate="onCalculate"
      @edit="onEditSample"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, watch, onMounted, onUnmounted } from 'vue'
import { connected } from './api/ws.js'
import {
  connect, listProjects, getAppState, openProject, saveProject, closeProject,
  getWaterSampleState, connectWaterSamples, disconnectWaterSamples,
  setAutoCalc, calculate, editSample,
  onProjects, onAppState, onProjectOpened, onProjectSaved, onProjectClosed,
  onIsChanged, onWaterSampleState, onWaterSampleConnected,
  onWaterSampleChanged, onWaterSampleCalculated, onAutoCalcSet,
  onOpen,
} from './api/ws.js'
import AppHeader from './components/AppHeader.vue'
import ProjectDialog from './components/ProjectDialog.vue'
import WaterSampleCalcPage from './pages/WaterSampleCalcPage.vue'
import type { CalcProjectProjection, WaterSampleProxy } from './api/schemas/responses/index.js'

const showProjectDialog = ref(false)
const projects = ref<CalcProjectProjection[]>([])
const projectName = ref('')
const isChanged = ref(false)
const samples = ref<WaterSampleProxy[]>([])
const isAutoCalc = ref(false)

const unsubs: (() => void)[] = []

watch(projectName, (val) => {
  if (val) {
    connectWaterSamples()
  } else {
    disconnectWaterSamples()
  }
})

onMounted(() => {
  connect()

  unsubs.push(onProjects(msg => {
    projects.value = msg.projects || []
  }))

  unsubs.push(onAppState(msg => {
    const state = msg.state
    if (state && state.project) {
      projectName.value = state.project.name
    } else {
      projectName.value = ''
    }
    isChanged.value = state ? state.isChanged : false
  }))

  unsubs.push(onProjectOpened(() => {
    getAppState()
    getWaterSampleState()
  }))

  unsubs.push(onProjectSaved(() => {
    getAppState()
  }))

  unsubs.push(onProjectClosed(() => {
    projectName.value = ''
    isChanged.value = false
    samples.value = []
  }))

  unsubs.push(onIsChanged(msg => {
    if (msg.isChanged === true || msg.isChanged === false) {
      isChanged.value = msg.isChanged
    }
  }))

  unsubs.push(onWaterSampleState(msg => {
    const state = msg.state
    if (state && state.list) {
      samples.value = state.list
    }
    if (state && state.isAutoCalc !== undefined) {
      isAutoCalc.value = state.isAutoCalc
    }
  }))

  unsubs.push(onWaterSampleConnected(() => {
    getWaterSampleState()
  }))

  unsubs.push(onWaterSampleChanged(msg => {
    if (msg && msg.key) {
      const idx = samples.value.findIndex(s => s.key === msg.key)
      if (idx !== -1) {
        samples.value = samples.value.map((s, i) =>
          i === idx ? { ...s, ...msg.properties, isOutdated: msg.isOutdated } as WaterSampleProxy : s
        )
      }
    }
  }))

  unsubs.push(onWaterSampleCalculated(() => {
    getWaterSampleState()
  }))

  unsubs.push(onAutoCalcSet(msg => {
    if (msg.isAuto !== undefined) {
      isAutoCalc.value = msg.isAuto
    }
  }))

  // После открытия соединения проверить состояние (восстановленная сессия)
  unsubs.push(onOpen(() => {
    getAppState()
    getWaterSampleState()
  }))
})

onUnmounted(() => {
  unsubs.forEach(fn => fn())
})

function selectProject(): void {
  showProjectDialog.value = true
  listProjects()
}

function onSave(): void {
  saveProject()
}

function onClose(): void {
  closeProject()
}

function onOpenProject(id: number): void {
  showProjectDialog.value = false
  openProject(id)
}

function onSetAutoCalc(value: boolean): void {
  isAutoCalc.value = value
  setAutoCalc(value)
}

function onCalculate(): void {
  calculate()
}

function onEditSample(key: string, property: string, value: number | null): void {
  editSample(key, property, value)
}
</script>
