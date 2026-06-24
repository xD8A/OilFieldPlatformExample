<template>
  <div class="table-pie-container">
    <div class="table-wrapper">
      <table class="sample-table" v-if="samples.length">
        <thead>
          <tr>
            <th rowspan="2" class="col-radio"></th>
            <th rowspan="2" class="col-info">Дата отбора</th>
            <th rowspan="2" class="col-info">Тип воды</th>
            <th rowspan="2" class="col-info">КНС/ДНС</th>
            <th rowspan="2" class="col-info">Скважина</th>
            <th colspan="4" class="col-group">Анионы, мг/л</th>
            <th colspan="3" class="col-group">Катионы, мг/л</th>
            <th colspan="4" class="col-group">Анионы, мг-экв/л</th>
            <th colspan="3" class="col-group">Катионы, мг-экв/л</th>
          </tr>
          <tr>
            <th>Cl⁻</th>
            <th>CO₃²⁻</th>
            <th>HCO₃⁻</th>
            <th>SO₄²⁻</th>
            <th>Ca²⁺</th>
            <th>Mg²⁺</th>
            <th>Na⁺</th>
            <th>Cl⁻</th>
            <th>CO₃²⁻</th>
            <th>HCO₃⁻</th>
            <th>SO₄²⁻</th>
            <th>Ca²⁺</th>
            <th>Mg²⁺</th>
            <th>Na⁺</th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="s in samples"
            :key="s.key"
            :class="{ outdated: s.isOutdated, selected: selectedKey === s.key }"
          >
            <td class="col-radio">
              <input type="radio" :value="s.key" v-model="selectedKey" />
            </td>
            <td>{{ formatDate(s.sampledAt) }}</td>
            <td>{{ waterTypeLabel(s.waterType) }}</td>
            <td>{{ s.clusterStationName ?? '' }}</td>
            <td>{{ s.wellName ?? '' }}</td>
            <td class="editable" @click="startEdit(s, 'chloride')">
              <template v-if="editKey === s.key + ':chloride'">
                <input ref="editors" type="number" step="any" :value="s.chloride" @change="finishEdit(s, 'chloride', $event)" @blur="finishEdit(s, 'chloride', $event)" @keydown.enter="finishEdit(s, 'chloride', $event)" @keydown.escape="editKey = null" />
              </template>
              <template v-else>{{ fmt(s.chloride) }}</template>
            </td>
            <td class="editable" @click="startEdit(s, 'carbonate')">
              <template v-if="editKey === s.key + ':carbonate'">
                <input type="number" step="any" :value="s.carbonate" @change="finishEdit(s, 'carbonate', $event)" @blur="finishEdit(s, 'carbonate', $event)" @keydown.enter="finishEdit(s, 'carbonate', $event)" @keydown.escape="editKey = null" />
              </template>
              <template v-else>{{ fmt(s.carbonate) }}</template>
            </td>
            <td class="editable" @click="startEdit(s, 'bicarbonate')">
              <template v-if="editKey === s.key + ':bicarbonate'">
                <input type="number" step="any" :value="s.bicarbonate" @change="finishEdit(s, 'bicarbonate', $event)" @blur="finishEdit(s, 'bicarbonate', $event)" @keydown.enter="finishEdit(s, 'bicarbonate', $event)" @keydown.escape="editKey = null" />
              </template>
              <template v-else>{{ fmt(s.bicarbonate) }}</template>
            </td>
            <td class="editable" @click="startEdit(s, 'sulfate')">
              <template v-if="editKey === s.key + ':sulfate'">
                <input type="number" step="any" :value="s.sulfate" @change="finishEdit(s, 'sulfate', $event)" @blur="finishEdit(s, 'sulfate', $event)" @keydown.enter="finishEdit(s, 'sulfate', $event)" @keydown.escape="editKey = null" />
              </template>
              <template v-else>{{ fmt(s.sulfate) }}</template>
            </td>
            <td class="editable" @click="startEdit(s, 'calcium')">
              <template v-if="editKey === s.key + ':calcium'">
                <input type="number" step="any" :value="s.calcium" @change="finishEdit(s, 'calcium', $event)" @blur="finishEdit(s, 'calcium', $event)" @keydown.enter="finishEdit(s, 'calcium', $event)" @keydown.escape="editKey = null" />
              </template>
              <template v-else>{{ fmt(s.calcium) }}</template>
            </td>
            <td class="editable" @click="startEdit(s, 'magnesium')">
              <template v-if="editKey === s.key + ':magnesium'">
                <input type="number" step="any" :value="s.magnesium" @change="finishEdit(s, 'magnesium', $event)" @blur="finishEdit(s, 'magnesium', $event)" @keydown.enter="finishEdit(s, 'magnesium', $event)" @keydown.escape="editKey = null" />
              </template>
              <template v-else>{{ fmt(s.magnesium) }}</template>
            </td>
            <td class="editable" @click="startEdit(s, 'sodium')">
              <template v-if="editKey === s.key + ':sodium'">
                <input type="number" step="any" :value="s.sodium" @change="finishEdit(s, 'sodium', $event)" @blur="finishEdit(s, 'sodium', $event)" @keydown.enter="finishEdit(s, 'sodium', $event)" @keydown.escape="editKey = null" />
              </template>
              <template v-else>{{ fmt(s.sodium) }}</template>
            </td>
            <td class="equiv">{{ fmt(s.chlorideEquivalent) }}</td>
            <td class="equiv">{{ fmt(s.carbonateEquivalent) }}</td>
            <td class="equiv">{{ fmt(s.bicarbonateEquivalent) }}</td>
            <td class="equiv">{{ fmt(s.sulfateEquivalent) }}</td>
            <td class="equiv">{{ fmt(s.calciumEquivalent) }}</td>
            <td class="equiv">{{ fmt(s.magnesiumEquivalent) }}</td>
            <td class="equiv">{{ fmt(s.sodiumEquivalent) }}</td>
          </tr>
        </tbody>
      </table>
      <p v-else class="empty">Нет проб воды</p>
    </div>
    <div class="pie-panel" v-if="selectedSample && hasEquivalents(selectedSample)">
      <h3 class="pie-title">Эквивалентный состав, мг-экв/л</h3>
      <svg :viewBox="`0 0 ${size} ${size}`" class="pie-svg">
        <g>
          <path v-for="(slice, i) in pieSlices(selectedSample)" :key="i"
            :d="slice.d" :fill="slice.color" stroke="#1e1e2e" stroke-width="1" />
        </g>
      </svg>
      <div class="pie-legend">
        <span v-for="(slice, i) in pieSlices(selectedSample)" :key="i" class="legend-item">
          <span class="legend-dot" :style="{ background: slice.color }"></span>
          {{ slice.label }}: {{ fmt(slice.value) }}
        </span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, nextTick } from 'vue'
import type { WaterSampleProxy } from '../api/schemas/responses/index.js'

const props = defineProps<{
  samples: WaterSampleProxy[]
  isAutoCalc: boolean
}>()

const emit = defineEmits<{
  edit: [key: string, property: string, value: number | null]
}>()

const editKey = ref<string | null>(null)
const editors = ref<HTMLElement | HTMLElement[] | null>(null)
const selectedKey = ref<string | null>(null)

const selectedSample = computed(() =>
  props.samples.find(s => s.key === selectedKey.value) ?? null
)

const colors = [
  '#f7768e', '#9ece6a', '#e0af68', '#7aa2f7',
  '#bb9af7', '#73daca', '#ff9e64',
]

interface EquivSlice {
  label: string
  value: number
  color: string
  d: string
}

const size = 200
const center = size / 2
const r = 90

function hasEquivalents(s: WaterSampleProxy): boolean {
  return s.chlorideEquivalent != null
    || s.carbonateEquivalent != null
    || s.bicarbonateEquivalent != null
    || s.sulfateEquivalent != null
    || s.calciumEquivalent != null
    || s.magnesiumEquivalent != null
    || s.sodiumEquivalent != null
}

function pieSlices(s: WaterSampleProxy): EquivSlice[] {
  const raw: { label: string; value: number; color: string }[] = [
    { label: 'Cl⁻', value: s.chlorideEquivalent ?? 0, color: colors[0] },
    { label: 'CO₃²⁻', value: s.carbonateEquivalent ?? 0, color: colors[1] },
    { label: 'HCO₃⁻', value: s.bicarbonateEquivalent ?? 0, color: colors[2] },
    { label: 'SO₄²⁻', value: s.sulfateEquivalent ?? 0, color: colors[3] },
    { label: 'Ca²⁺', value: s.calciumEquivalent ?? 0, color: colors[4] },
    { label: 'Mg²⁺', value: s.magnesiumEquivalent ?? 0, color: colors[5] },
    { label: 'Na⁺', value: s.sodiumEquivalent ?? 0, color: colors[6] },
  ]

  const total = raw.reduce((a, b) => a + b.value, 0)
  if (total === 0) return []

  let cumAngle = -Math.PI / 2
  return raw
    .filter(e => e.value > 0)
    .map(e => {
      const sliceAngle = (e.value / total) * 2 * Math.PI
      const x1 = center + r * Math.cos(cumAngle)
      const y1 = center + r * Math.sin(cumAngle)
      const x2 = center + r * Math.cos(cumAngle + sliceAngle)
      const y2 = center + r * Math.sin(cumAngle + sliceAngle)
      const large = sliceAngle > Math.PI ? 1 : 0
      const d = `M${center},${center}L${x1.toFixed(1)},${y1.toFixed(1)}A${r},${r} 0 ${large} 1 ${x2.toFixed(1)},${y2.toFixed(1)}Z`
      cumAngle += sliceAngle

      return { label: e.label, value: e.value, color: e.color, d } as EquivSlice
    })
}

function startEdit(sample: WaterSampleProxy, property: string): void {
  editKey.value = sample.key + ':' + property
  nextTick(() => {
    if (editors.value) {
      const el = Array.isArray(editors.value) ? editors.value[0] : editors.value
      el?.focus()
    }
  })
}

function finishEdit(sample: WaterSampleProxy, property: string, event: FocusEvent | KeyboardEvent): void {
  if (editKey.value === null) return
  const target = event.target as HTMLInputElement
  const raw = target.value
  const value = raw === '' ? null : parseFloat(raw)
  editKey.value = null
  if (value !== (sample as Record<string, any>)[property]) {
    emit('edit', sample.key, property, value)
  }
}

function fmt(val: number | null | undefined): string {
  if (val === null || val === undefined) return ''
  return Number(val).toFixed(2)
}

function formatDate(dateStr: string | null | undefined): string {
  if (!dateStr) return ''
  const d = new Date(dateStr)
  return d.toLocaleDateString('ru-RU')
}

function waterTypeLabel(type: number): string {
  if (type === 1) return 'Пластовая'
  if (type === 2) return 'Закачиваемая'
  return String(type)
}
</script>
