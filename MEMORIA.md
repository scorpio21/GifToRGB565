# Memoria — GifRGB565GUI

## Preferencias del usuario
- **Idioma:** Responder SIEMPRE en español.
- **Versiones:** No cambiar de version hasta que el usuario lo indique. Todos los cambios van en v1.1.4.

## Estado actual
- Proyecto: GifRGB565GUI (C# WinForms .NET 8.0-windows)
- Versión actual: v1.1.4
- GitHub repo: scorpio21/GifToRGB565
- Issues abiertos pendientes: #28, #29, #30, #31, #32, #33, #34, #35, #36
- Issues cerrados: #1–#27 (todos cerrados)

## Issues abiertas (mejoras pendientes)
| # | Título | Prioridad |
|---|--------|-----------|
| 28 | Reemplazar GetPixel/SetPixel por LockBits en ImageConverter | CRITICAL |
| 29 | SaveFileDialog para export .h | ALTA |
| 30 | Persistir preferencias (formato, filtros, velocidad, loop) | MEDIA |
| 31 | LoadGif: disposal + advertencia memoria | ALTA |
| 32 | Reemplazar JSON hand-rolled con System.Text.Json | MEDIA |
| 33 | Reordenar: mantener índice en vez de renombrar archivos | MEDIA |
| 34 | DebugForm: fix leak diffBmp + optimizar alloc | MEDIA |
| 35 | CompareForm wipe: reutilizar bitmap | BAJA |
| 36 | Accesibilidad: TabIndex, AccessibleName, i18n | BAJA |

## Funcionalidades completadas
- v1.0: Conversión GIF/frames/header → RGB565, exportación N64/BIN/BINGZ, dithering, reducción ruido, enfoque, gzip
- v1.1.0: Auto-sanitize nombre salida, persistencia config, menú reciente (max 5), fix mostrar nombre archivo
- v1.1.1: Dark Mode con persistencia, barra de estado (dimensiones, frames, tamaño, formato)
- v1.1.2: ResizeForm con presets y remember settings (#12, #16)
- v1.1.3: Atajos de teclado, Comparar imágenes, Acerca de (#13, #14, #15)
- v1.1.4: Async/await (#18), Reorder/eliminar frames (#20), Export frames con formato (#21), Config en AppData (#22), RGB565 preview (#23), StatusStrip theme (#24), CompareForm animation (#26), File lock fixes, Batch processing (#19), Debug RGB565 (#27)

## Archivos importantes
- `Form1.cs`: ~2140 líneas, lógica principal (async generate, batch queue, reorder, RGB565 preview, export)
- `Form1.Designer.cs`: ~875 líneas, diseño UI (controles batch, iconos, menús)
- `ImageConverter.cs`: 193 líneas, motor conversión RGB565 (GetPixel/SetIssue pendiente #28)
- `ThemeManager.cs`: Clase estática para temas oscuro/claro (recursive, Tag="accent", StatusStrip)
- `ResizeForm.cs`: Redimensionar con Magick.NET, presets, remember settings
- `CropForm.cs`: Recortar con handles arrastrables, aspect lock, autocrop
- `CompareForm.cs`: Comparar original vs RGB565 (side-by-side, wipe, overlay, animación)
- `RGB565DebugForm.cs`: Debug canales R/G/B, zoom pixel a pixel, diff, opacidad
- `last_output.json`: Config en `%AppData%\GifToRGB565\`
- `resize_settings.json`: Settings ResizeForm en `%AppData%\GifToRGB565\`

## Notas técnicas importantes
- **Magick.NET v14:** `AnimationDelay` retorna `uint`, `MagickGeometry` sin `Gravity`, `Gravity` en `MagickImage`, `Page` en vez de `RePage`
- **File locks:** Todo `Image.FromFile` reemplazado por `LoadBitmapUnlocked()` (File.ReadAllBytes → MemoryStream → Bitmap)
- **Async:** Zero `Application.DoEvents()`, todo con `Task.Run` + `CancellationToken`
- **ThemeManager:** Soporta `Tag="accent"`, StatusStrip, ToolStripStatusLabel, Panel, ComboBox, recursive
- **Config:** Ahora en `%AppData%\GifToRGB565\` (no en AppContext.BaseDirectory)

## Proyecto secundario
- **cambiar_iconos** (`E:/xampp/htdocs/cambiar_iconos`): WinForms icon resizer, GitHub repo scorpio21/cambiar_iconos

## Herramientas
- gh CLI: `C:\Program Files\GitHub CLI\gh.exe`, autenticado como scorpio21
- Labels: `priority: alta` (B60205), `priority: media` (FBCA04), `priority: baja` (0E8A16), `bug`, `enhancement`
