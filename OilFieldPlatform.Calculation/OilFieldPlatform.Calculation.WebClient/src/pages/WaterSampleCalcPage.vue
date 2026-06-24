<template>
  <div class="workspace" v-if="projectName">
    <div class="toolbar">
      <label class="auto-calc">
        <input type="checkbox" :checked="isAutoCalc" @change="$emit('setAutoCalc', ($event.target as HTMLInputElement).checked)" />
        Авторасчёт
      </label>
      <button class="btn" @click="$emit('calculate')" :disabled="isAutoCalc">Рассчитать</button>
      <span class="auto-calc-indicator" v-if="isAutoCalc">авторасчёт включён</span>
    </div>
    <WaterSampleTable
      :samples="samples"
      :is-auto-calc="isAutoCalc"
      @edit="(key, property, value) => $emit('edit', key, property, value)"
    />
    <LogPanel :logs="logs" />
  </div>
  <div class="workspace-empty" v-else>
    <p v-if="!connected">Подключение к серверу...</p>
    <p v-else>Откройте проекта для начала работы</p>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import WaterSampleTable from '../components/WaterSampleTable.vue'
import LogPanel from '../components/LogPanel.vue'
import type { WaterSampleProxy, LogResponse } from '../api/schemas/responses/index.js'
import { onLog } from '../api/ws.js'

defineProps<{
  projectName: string
  isAutoCalc: boolean
  samples: WaterSampleProxy[]
  connected: boolean
}>()

defineEmits<{
  setAutoCalc: [value: boolean]
  calculate: []
  edit: [key: string, property: string, value: number | null]
}>()

const logs = ref<LogResponse[]>([])

const unsubs: (() => void)[] = []

onMounted(() => {
  unsubs.push(onLog(msg => {
    logs.value = [...logs.value.slice(-9), msg]
  }))
})

onUnmounted(() => {
  unsubs.forEach(fn => fn())
})
</script>
