# Changelog — GifRGB565GUI

## [1.1.0] — 2026-09-01

### Added
- **Auto-sanitize de nombre de salida:** Los caracteres inválidos se eliminan automáticamente en vez de mostrar error. Se actualiza el textbox y se muestra en el log el cambio realizado.
- **Persistencia de nombre de salida:** El último nombre de salida se guarda en `last_output.json` y se carga al abrir la aplicación.
- **Menú "Abierto reciente":** Submenú dentro de "Archivo" con las últimas 5 rutas de GIFs, carpetas o headers abiertos. Incluye "Borrar historial".
- **`SanitizeFileName()`:** Método que elimina caracteres inválidos según `Path.GetInvalidFileNameChars()` y separadores de ruta.
- **`AddToRecentFiles()`:** Gestiona el historial de archivos, manteniendo un máximo de 5 entradas.
- **`LoadRecentMenu()`:** Construye dinámicamente el submenú de recientes.
- **`SaveConfig()` / `LoadConfig()`:** Persistencia de configuración en JSON.

### Changed
- **`btnGenerate_Click`:** Reemplaza bloque de validación por sanitizado automático. Guarda el nombre tras sanitizar.
- **`btnSelectFolder_Click`:** Registra la ruta completa del archivo en el historial (no solo la carpeta).
- **`cargarHeaderToolStripMenuItem_Click`:** Registra el header cargado en el historial.
- **`Form1_Load`:** Carga el nombre guardado y el menú de recientes al iniciar.

### Fixed
- **Historial mostraba solo la carpeta:** Al abrir un archivo individual (ej: PNG), ahora se muestra el nombre del archivo en vez del nombre de la carpeta.

---

## [1.0.0] — Versión inicial

- Conversión de GIF animado a RGB565 para N64/ESP32.
- Exportación a `.h` (N64), `.bin` y `.bin.gz` (ESP32).
- Opciones de dithering, reducción de ruido y sharpening.
- Carga de headers `.h` existentes.
- Simulación de visualización en display.
- Interfaz con preview de frames, animación y controles de velocidad.
