<template>
  <div class="log-section" v-if="logs.length">
    <h3 class="log-title">Лог сервера</h3>
    <table class="log-table">
      <thead>
        <tr>
          <th class="col-level">Уровень</th>
          <th>Сообщение</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(entry, i) in logs" :key="i" :class="'log-' + entry.level.toLowerCase()">
          <td class="col-level">{{ entry.level }}</td>
          <td>{{ entry.message }}</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  logs: { level: string; message: string; exception?: string }[]
}>()
</script>

<style scoped>
.log-section {
  border-top: 1px solid var(--color-border);
  padding: 0.5rem 1rem;
}

.log-title {
  font-size: 0.85rem;
  font-weight: 600;
  margin-bottom: 0.4rem;
}

.log-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.78rem;
  font-family: monospace;
}

.log-table th,
.log-table td {
  border: 1px solid var(--color-border);
  padding: 0.2rem 0.4rem;
  text-align: left;
}

.log-table th {
  background: var(--color-header-bg);
  font-weight: 600;
}

.col-level {
  width: 6rem;
  white-space: nowrap;
}

.log-warning td:first-child {
  color: var(--color-warning);
}

.log-error td:first-child {
  color: var(--color-danger);
}

.log-critical td:first-child {
  color: var(--color-danger);
  font-weight: bold;
}
</style>
