# Memoria — GifRGB565GUI

## Preferencias del usuario
- **Idioma:** Responder SIEMPRE en español.

## Estado actual
- Proyecto: GifRGB565GUI (C# WinForms .NET 8.0-windows)
- Versión actual: v1.1.1
- GitHub repo: scorpio21/GifToRGB565
- Último commit: 7279b17
- Issues abiertos pendientes: #1, #2, #3, #4, #5, #6, #7, #9, #11
- Issues cerrados: #8 (Dark Mode), #10 (Barra de estado)

## Funcionalidades completadas
- v1.0: Conversión GIF/frames/header → RGB565, exportación N64/BIN/BINGZ, dithering, reducción ruido, enfoque, gzip
- v1.1.0: Auto-sanitize nombre salida, persistencia config, menú reciente (max 5), fix mostrar nombre archivo
- v1.1.1: Dark Mode con persistencia, barra de estado (dimensiones, frames, tamaño, formato)

## Archivos importantes
- `ThemeManager.cs`: Clase estática para temas oscuro/claro
- `last_output.json`: Persistencia de config (nombre, tema, recientes)
- `Form1.cs`: ~1160 líneas, lógica principal
- `Form1.Designer.cs`: ~570 líneas, diseño UI
- `ImageConverter.cs`: Motor de conversión RGB565 (193 líneas)

## Próximos pasos (issues abiertos)
- #1: Exportación a array C#
- #2: Frame rate personalizado
- #3: Previsualización de animación
- #4: Exportación individual de frames
- #5: Soporte WebP/AVIF
- #6: Escalado de imagen
- #7: Filtro personalizado
- #9: Optimización de memoria
- #11: Botón de ayuda contextual
