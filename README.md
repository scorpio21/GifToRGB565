# GifToRGB565

Conversor de GIF / secuencia de imágenes a arrays RGB565 (formato para N64) con interfaz gráfica en WinForms.

Características
- Cargar GIF animado o carpeta de frames (`.png`, `.jpg`).
- Previsualizar frames y reproducir animación con control de velocidad y loop.
- Generar archivo `n64.h` con los frames convertidos a formato RGB565.
- Opciones de procesamiento: dithering, noise reduction y sharpen.

Requisitos
- .NET 8
- Visual Studio 2022/2023 u otro IDE compatible con WinForms y .NET 8

Instalación y uso
1. Clona el repositorio:
   `git clone https://github.com/scorpio21/GifToRGB565.git`
2. Abre la solución `GifRGB565GUI.slnx` en Visual Studio.
3. Restaura paquetes y compila la solución.
4. Ejecuta la aplicación, selecciona un GIF o una carpeta de frames y usa los controles para previsualizar y generar `output/n64.h`.

Contribuir
- Abre un _issue_ para reportar errores o proponer mejoras.
- Crea _pull requests_ con descripciones claras de los cambios.
- Sigue el estilo de código existente; escribe mensajes de commit claros.

Estructura del proyecto
- `GifRGB565GUI/` - Proyecto WinForms con el código fuente.
- `output/` - Carpeta generada en tiempo de ejecución donde se escribe `n64.h`.

Licencia
Este proyecto está bajo la licencia MIT. Consulta el archivo `LICENSE` para más detalles.

Contacto
- Autor: scorpio21 (https://github.com/scorpio21)

Notas
- Si encuentras problemas con la reproducción de la animación revisa que el `Timer` está asociado a `animTimer_Tick` en `Form1.Designer.cs` y que el intervalo del `Timer` no sea 0.
- El proyecto incluye un conversor `ImageConverter` para generar los datos RGB565; revisa su implementación si necesitas adaptar la salida a otros formatos.
