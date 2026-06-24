<template>
  <header class="header">
    <div class="header-left">
      <span class="header-title">OilFieldPlatform</span>
      <span class="project-name" v-if="projectName">
        {{ projectName }}
        <span class="changed-indicator" v-if="isChanged" title="Есть несохранённые изменения">*</span>
      </span>
      <span class="project-name muted" v-else>нет открытого проекта</span>
    </div>
    <div class="header-right">
      <span class="connection-status" :class="{ connected }">
        {{ connected ? 'WS ✓' : 'WS ✗' }}
      </span>
      <button class="btn" @click="$emit('select')">Выбрать</button>
      <button class="btn" @click="$emit('save')" :disabled="!projectName || !isChanged">Сохранить</button>
      <button class="btn" @click="$emit('close')" :disabled="!projectName">Закрыть</button>
    </div>
  </header>
</template>

<script setup lang="ts">
defineProps<{
  projectName: string
  isChanged: boolean
  connected: boolean
}>()

defineEmits<{
  select: []
  save: []
  close: []
}>()
</script>
