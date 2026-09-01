# Changelog — GifRGB565GUI

---

## [1.1.0] — 2026-09-01

### Added — Nombre de salida
- **Auto-sanitize:** Los caracteres inválidos se eliminan automáticamente en vez de mostrar error. Se actualiza el textbox y se muestra en el log el cambio realizado.
- **Persistencia:** El último nombre de salida se guarda en `last_output.json` y se carga al abrir la aplicación.
- **`SanitizeFileName()`:** Elimina caracteres inválidos según `Path.GetInvalidFileNameChars()` y separadores de ruta.
- **`SaveConfig()` / `LoadConfig()`:** Persistencia de configuración en JSON.

### Added — Menú "Abierto reciente"
- Submenú dentro de "Archivo" con las últimas 5 rutas de GIFs, carpetas o headers abiertos.
- Tooltip con ruta completa en cada entrada.
- **"Borrar historial":** Limpia todas las entradas del historial.
- **Detección de archivos eliminados:** Si un archivo ya no existe, avisa y lo elimina del historial.
- **`AddToRecentFiles()`:** Gestiona el historial, manteniendo un máximo de 5 entradas.
- **`LoadRecentMenu()`:** Construye dinámicamente el submenú de recientes.
- **`LoadRecentFiles()` / `SaveRecentFiles()`:** Lectura y escritura del historial en JSON.

### Changed
- **`btnGenerate_Click`:** Reemplaza bloque de validación por sanitizado automático. Guarda el nombre tras sanitizar.
- **`btnSelectFolder_Click`:** Registra la ruta completa del archivo en el historial (no solo la carpeta).
- **`cargarHeaderToolStripMenuItem_Click`:** Registra el header cargado en el historial.
- **`Form1_Load`:** Carga el nombre guardado y el menú de recientes al iniciar.

### Fixed
- **Historial mostraba solo la carpeta:** Al abrir un archivo individual (ej: PNG), ahora se muestra el nombre del archivo en vez del nombre de la carpeta.

---

## [1.0.0] — Versión inicial

### Carga de archivos
- **GIF animado:** Carga archivos `.gif`, extrae todos los frames individuales.
- **Carpeta de frames:** Carga imágenes `.png` y `.jpg` desde una carpeta.
- **Filtro de apertura:** "GIF Animado (*.gif) | Carpeta de frames (*.*)."

### Procesamiento de imagen
- **Conversión a RGB565:** Convierte píxeles de 24-bit a formato RGB565 (16-bit, 5-6-5 bits).
- **Dithering Floyd-Steinberg:** Distribuye el error de cuantización a píxeles vecinos para suavizar bandas de color.
- **Reducción de ruido:** Filtro de suavizado con kernel Gaussiano 3x3.
- **Sharpen (realce):** Filtro de realce de bordes con kernel 3x3.
- **`ImageConverter.ToRGB565()`:** Conversión con opciones configurables de pre-procesamiento.
- **`Clamp()`:** Limita valores de píxel al rango 0-255.

### Formatos de exportación
- **N64 header (.h):** Genera archivo C con array `const unsigned short PROGMEM n64[frames][width*height]`.
- **ESP32 bin (.bin):** Archivo binario con header (width, height, frames como int32 LE) + datos RGB565 uint16 LE.
- **ESP32 bin.gz (.bin.gz):** Misma exportación comprimida con GZip.
- **`GenerateHeaderAtPath()`:** Genera el header C para N64.
- **`ExportBin()`:** Genera el archivo binario con opción de compresión GZip.
- **`ValidateFrameSizes()`:** Verifica que todos los frames tengan el mismo tamaño.

### Reproducción y previsualización
- **Lista de frames:** ListBox con nombres de frames, selección manual.
- **Preview:** PictureBox con imagen del frame seleccionado.
- **Play/Stop:** Reproducción automática de la animación.
- **Next/Previous:** Navegación frame a frame con botones habilitados/deshabilitados según posición.
- **Velocidad:** TrackBar para ajustar intervalo de reproducción (en milisegundos).
- **Loop:** Checkbox para activar/desactivar repetición de la animación.
- **`animTimer_Tick`:** Lógica de reproducción con soporte de loop y cambio de frames.

### Simulación RGB565
- **`btnSimulate_Click`:** Abre ventana modal que reproduce la animación convertida a RGB565.
- **`ConvertRgb565ToBitmap()`:** Convierte datos RGB565 de vuelta a Bitmap 24-bit para visualización.
- **`ShowSimFromRgb565()`:** Simulación desde datos RGB565 en memoria (headers cargados).

### Utilidades
- **Cargar header (.h):** Parsea archivos header existentes y carga los frames RGB565.
- **`ParseHeaderFile()`:** Parser flexible que extrae width, height, frames y datos hex del header C.
- **Exportar todos los frames:** Exporta todos los frames como archivos PNG individuales a una carpeta seleccionada.
- **`exportarFramesToolStripMenuItem_Click`:** Exporta frames como `frame_000.png`, `frame_001.png`, etc.

### Interfaz de usuario
- **Menú principal:**
  - Archivo → Abierto reciente → Salir
  - Compresión → n64.h / esp32.bin / esp32.bin.gz
  - Utilidades → Cargar .h / Exportar todos los Frames
  - Ayuda → Dithering / Noise Reduction / Sharpen / GZip / Acerca de
- **Barra de progreso:** Muestra avance durante la conversión de frames.
- **Panel de log:** Muestra mensajes de operación en tiempo real.
- **Checkboxes de opciones:** Dithering, Noise Reduction, Sharpen — con estado visual (activado/desactivado).
- **Nombre de salida:** TextBox para especificar el nombre del archivo de salida.
- **`UpdateGenerateButtonText()`:** Actualiza texto del botón según formato seleccionado.
- **`Log()`:** Método auxiliar para escribir en el panel de log.

### Ayuda
- **Dithering:** Explica el uso del patrón para mitigar bandas de color.
- **Noise Reduction:** Explica el filtrado de ruido y su efecto en el detalle fino.
- **Sharpen:** Explica el realce de bordes antes de la conversión.
- **GZip:** Explica la compresión y su impacto en tamaño/CPU.
- **Acerca de:** Muestra versión, repositorio y autor.

### Arquitectura técnica
- **Target Framework:** .NET 8.0 (Windows Forms)
- **Archivos principales:**
  - `Form1.cs` — Lógica principal de la aplicación (1080 líneas)
  - `Form1.Designer.cs` — Diseño de interfaz auto-generado
  - `ImageConverter.cs` — Motor de conversión RGB565 (193 líneas)
  - `Program.cs` — Punto de entrada
- **Formato de configuración:** JSON simple (`last_output.json`)
- **Dependencias:** Solo librerías estándar de .NET (System.Drawing, System.IO.Compression)
