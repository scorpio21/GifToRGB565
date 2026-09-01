# Especificación: Menú "Abierto reciente"

**Fecha:** 2026-09-01
**Proyecto:** GifRGB565GUI
**Estado:** Aprobado

---

## 1. Resumen

Añadir un submenú "Abierto reciente" dentro de "Archivo" que muestra las últimas 5 rutas de GIFs o carpetas abiertas, permitiendo reabrirlas con un clic.

---

## 2. Contexto actual

**Menú actual (Form1.Designer.cs):**
- Archivo → Salir
- Compresión → n64.h / esp32.bin / esp32.bin.gz
- Utilidades → Cargar .h / Exportar todos los Frames
- Ayuda → ...

**Apertura de archivos (Form1.cs:129):**
- `btnSelectFolder_Click` usa `OpenFileDialog` con filtro "GIF Animado|*.gif|Carpeta de frames|*.*"
- Si es `.gif` → `LoadGif(path)`
- Si no → toma la carpeta y `LoadFrames()`

---

## 3. Diseño

### 3.1 Estructura del menú

```
Archivo
├── Abierto reciente    ►  ├── archivo1.gif
│                          ├── MiCarpeta
│                          ├── otro.gif
│                          ├── ───────────
│                          └── Borrar historial
├── ───────────
└── Salir
```

- Máximo **5 entradas** (las más recientes primero)
- Separador + "Borrar historial" al final
- Cada entrada: nombre del archivo/carpeta como texto
- Tooltip: ruta completa

### 3.2 Persistencia

**Archivo:** `last_output.json` (mismo que ya existe junto al ejecutable)

**Estructura:**
```json
{
  "lastName": "mi_archivo",
  "recentFiles": [
    "C:\\Users\\...\\animacion.gif",
    "D:\\frames\\ejemplo"
  ]
}
```

- `recentFiles`: array de strings, máximo 5 elementos
- Al añadir un archivo que ya existe → se mueve al principio
- Al superar 5 → se elimina el último

### 3.3 Archivos a modificar

| Archivo | Cambio |
|---------|--------|
| `Form1.Designer.cs` | Añadir `recentToolStripMenuItem` como submenú de `archivoToolStripMenuItem` |
| `Form1.cs` | Añadir métodos `AddToRecentFiles()`, `LoadRecentMenu()`, `ClearRecentHistory()`, `recentFileItem_Click()` |

### 3.4 Métodos nuevos en Form1.cs

**`AddToRecentFiles(string path)`**
```csharp
private void AddToRecentFiles(string path)
{
    var recent = LoadRecentFiles();
    recent.Remove(path);           // si ya existía, mover al inicio
    recent.Insert(0, path);
    if (recent.Count > 5)
        recent.RemoveAt(5);
    SaveRecentFiles(recent);
    LoadRecentMenu();
}
```

**`LoadRecentFiles()` → `List<string>`**
- Lee `recentFiles` de `last_output.json`
- Si el archivo no existe → lo elimina de la lista

**`SaveRecentFiles(List<string> list)`**
- Escribe el array en `last_output.json` preservando `lastName`

**`LoadRecentMenu()`**
- Limpia items actuales del submenú
- Para cada ruta → añade `ToolStripMenuItem` con:
  - Text = nombre del archivo/carpeta
  - ToolTipText = ruta completa
  - Tag = ruta completa
  - Click += recentFileItem_Click
- Si hay más de 0 → añade separador + "Borrar historial"

**`recentFileItem_Click(sender, e)`**
- Obtiene la ruta de `((ToolStripMenuItem)sender).Tag`
- Si el archivo existe → lo carga (misma lógica que `btnSelectFolder_Click`)
- Si no existe → avisa y lo elimina del historial

**`clearRecentToolStripMenuItem_Click(sender, e)`**
- Vacía la lista y actualiza el menú

### 3.5 Puntos de integración

En `btnSelectFolder_Click` (línea 136), tras cargar exitosamente:
```csharp
AddToRecentFiles(path);
```

En `cargarHeaderToolStripMenuItem_Click`, tras cargar exitosamente:
```csharp
AddToRecentFiles(path);
```

En `Form1_Load`, añadir:
```csharp
LoadRecentMenu();
```

---

## 4. Flujo

```
 Usuario abre GIF/carpeta
        │
        ▼
 AddToRecentFiles(path)
        │
        ▼
 ¿Ya está en la lista? ──Sí──→ Mover al inicio
        │No
        ▼
 Insertar al inicio
        │
        ▼
 ¿Más de 5? ──Sí──→ Eliminar último
        │No
        ▼
 Guardar en JSON
        │
        ▼
 Actualizar submenú
```

---

## 5. Criterios de aceptación

- [ ] Submenú "Abierto reciente" visible en Archivo
- [ ] Al abrir un GIF/carpeta → se añade al historial
- [ ] Máximo 5 entradas, las más recientes primero
- [ ] Clic en una entrada → carga el archivo
- [ ] Si el archivo no existe → avisa y lo elimina del historial
- [ ] "Borrar historial" limpia todo
- [ ] Tooltip muestra la ruta completa
- [ ] Persiste entre sesiones (guardado en JSON)

---

## 6. No incluido

- Abrir archivos .h desde el historial
- Número configurable de entradas
- Historial por tipo de archivo separado
