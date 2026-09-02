# GifToRGB565

Conversor de GIF / secuencia de imágenes a arrays RGB565 (formato para N64/ESP32) con interfaz gráfica en WinForms.

## Características
- Cargar GIF animado o carpeta de frames (`.png`, `.jpg`).
- Previsualizar frames y reproducir animación con control de velocidad y loop.
- Generar archivo `n64.h` con los frames convertidos a formato RGB565.
- Exportar binarios para ESP32 (`.bin` y `.bin.gz`).
- Opciones de procesamiento: dithering, noise reduction y sharpen.
- **Rescale automático:** Redimensionar frames antes de convertir con presets (50%, 25%, 160x120, 320x240) o entrada manual, con opción de mantener proporción.
- **Barra de estado:** Muestra dimensiones del frame, total de frames, tamaño estimado del output y formato de exportación.
- **Tema oscuro/claro:** Cambio de tema desde Menú → Ver → Tema, con persistencia.
- **Menú "Abierto reciente":** Últimas 5 rutas abiertas (GIFs, carpetas, headers).
- Menú "Archivo" y "Compresión" para seleccionar formato de exportación.
- Barra de progreso y logs durante la conversión.

## Requisitos
- .NET 8
- Visual Studio 2022/2023 u otro IDE compatible con WinForms y .NET 8

## Instalación y uso
1. Clona el repositorio:
   `git clone https://github.com/scorpio21/GifToRGB565.git`
2. Abre la solución `GifRGB565GUI.slnx` en Visual Studio.
3. Restaura paquetes y compila la solución.
4. Ejecuta la aplicación.

## Uso rápido
- Selecciona un GIF animado o una carpeta de frames con `Seleccionar GIF/Carpeta`.
- El panel izquierdo lista los frames; puedes reproducir la animación con `Play` / `Stop`.
- En el menú superior selecciona `Compresión` y elige el formato de exportación:
  - `n64.h (original)` — genera el header `n64.h` en `output/` al pulsar `Generar`.
  - `esp32.bin` — al pulsar `Generar` se pregunta la ruta de salida para un `.bin`.
  - `esp32.bin.gz` — al pulsar `Generar` se pregunta la ruta de salida para un `.bin.gz`.
- Usa el grupo **Rescale** para cambiar la resolución de los frames antes de convertir.
- La barra `GZip (if applicable)` se puede usar para forzar compresión al exportar `.bin`.
- `Simular .h` abre una ventana que reproduce la animación convertida desde RGB565.

## Notas
- La barra de progreso se actualiza por cada frame procesado.
- La configuración (nombre de salida, tema, recientes, rescale) se guarda automáticamente en `last_output.json`.

## Contribuir
- Abre un _issue_ para reportar errores o proponer mejoras.
- Crea _pull requests_ con descripciones claras de los cambios.

## Licencia
Este proyecto está bajo la licencia MIT. Consulta el archivo `LICENSE` para más detalles.

## Contacto
- Autor: scorpio21 (https://github.com/scorpio21)
