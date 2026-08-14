# GifToRGB565

Conversor de GIF / secuencia de imágenes a arrays RGB565 (formato para N64) con interfaz gráfica en WinForms.

Características
- Cargar GIF animado o carpeta de frames (`.png`, `.jpg`).
- Previsualizar frames y reproducir animación con control de velocidad y loop.
- Generar archivo `n64.h` con los frames convertidos a formato RGB565.
- Exportar binarios para ESP32 (`.bin` y `.bin.gz`).
- Opciones de procesamiento: dithering, noise reduction y sharpen.
- Menú "Archivo" y "Compresión" para seleccionar formato de exportación.
- Barra de progreso y logs durante la conversión.

Requisitos
- .NET 8
- Visual Studio 2022/2023 u otro IDE compatible con WinForms y .NET 8

Instalación y uso
1. Clona el repositorio:
   `git clone https://github.com/scorpio21/GifToRGB565.git`
2. Abre la solución `GifRGB565GUI.slnx` en Visual Studio.
3. Restaura paquetes y compila la solución.
4. Ejecuta la aplicación.

Uso rápido
- Selecciona un GIF animado o una carpeta de frames con `Seleccionar GIF/Carpeta`.
- El panel izquierdo lista los frames; puedes reproducir la animación con `Play` / `Stop`.
- En el menú superior selecciona `Compresión` y elige el formato de exportación:
  - `n64.h (original)` — genera el header `n64.h` en `output/` al pulsar `Generar`.
  - `esp32.bin` — al pulsar `Generar` se pregunta la ruta de salida para un `.bin`.
  - `esp32.bin.gz` — al pulsar `Generar` se pregunta la ruta de salida para un `.bin.gz`.
- La barra `GZip (if applicable)` se puede usar para forzar compresión al exportar `.bin`.
- `Simular .h` abre una ventana que reproduce la animación convertida desde RGB565.

Notas
- La barra de progreso se actualiza por cada frame procesado.
- Si el diseñador de Visual Studio muestra errores en tiempo de diseño, cierra/reabre el archivo o reinicia Visual Studio para recargar la clase parcial (`Form1.Designer.cs` / `Form1.cs`).

Contribuir
- Abre un _issue_ para reportar errores o proponer mejoras.
- Crea _pull requests_ con descripciones claras de los cambios.

Licencia
Este proyecto está bajo la licencia MIT. Consulta el archivo `LICENSE` para más detalles.

Contacto
- Autor: scorpio21 (https://github.com/scorpio21)
