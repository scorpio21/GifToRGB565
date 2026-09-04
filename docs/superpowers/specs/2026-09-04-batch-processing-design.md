# Batch Processing — Diseño #19

## Objetivo
Permitir cargar múltiples GIFs en una cola y procesarlos secuencialmente con progreso global.

## UI (nuevos controles)
- `btnAddToQueue` — "Agregar a cola" (junto a btnSelectFolder)
- `lstQueue` — ListBox con archivos encolados (nombre + tamaño)
- `btnRemoveFromQueue` — "Quitar" de la cola
- `btnClearQueue` — "Limpiar cola"
- `btnProcessQueue` — "Procesar cola" (visible solo cuando hay archivos)
- `lblQueueProgress` — "Procesando 2/5..."

## Flujo
1. Click "Agregar a cola" → OpenFileDialog multiselect → agrega GIFs a lstQueue
2. Se puede agregar múltiples veces
3. Click "Procesar cola" → procesa secuencialmente
4. Cada GIF: carga → filtros → genera con formato actual
5. Salida: un archivo por GIF (nombre = archivo entrada + ext)
6. Progreso global en barra + log
7. "Cancelar" detiene la cola

## Config reutilizada
- Formato actual (.h / .bin / .bin.gz)
- Filtros (dither, noise, sharpen)
- GZip level
- Directorio de salida = carpeta del primer archivo

## No hace
- No mezcla GIFs en un solo archivo
- No configuración por archivo
- No reordenar cola
