<template>
  <div class="dialog-overlay" @click.self="$emit('close')">
    <div class="dialog">
      <div class="dialog-header">
        <h2>Выбор проекта</h2>
        <button class="btn-close" @click="$emit('close')">&times;</button>
      </div>
      <div class="dialog-body">
        <table class="project-table" v-if="projects.length">
          <thead>
            <tr>
              <th></th>
              <th>ID</th>
              <th>Наименование</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="p in projects"
              :key="p.id"
              :class="{ selected: selectedId === p.id }"
              @click="selectedId = p.id"
              @dblclick="openProject(p.id)"
            >
              <td><input type="radio" :value="p.id" v-model="selectedId" /></td>
              <td>{{ p.id }}</td>
              <td>{{ p.name }}</td>
            </tr>
          </tbody>
        </table>
        <p v-else class="empty">Нет доступных проектов</p>
      </div>
      <div class="dialog-footer">
        <button class="btn" @click="openProject(selectedId)" :disabled="!selectedId">Открыть</button>
        <button class="btn" @click="$emit('close')">Отмена</button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import type { CalcProjectProjection } from '../api/schemas/responses/index.js'

defineProps<{
  projects: CalcProjectProjection[]
}>()

const emit = defineEmits<{
  open: [id: number]
  close: []
}>()

const selectedId = ref<number | null>(null)

function openProject(id: number | null) {
  if (id) emit('open', id)
}
</script>
