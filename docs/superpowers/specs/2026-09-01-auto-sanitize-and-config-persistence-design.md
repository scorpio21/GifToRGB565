# Especificación: Auto-sanitize de nombre + Persistencia de configuración

**Fecha:** 2026-09-01
**Proyecto:** GifRGB565GUI
**Estado:** Borrador — pendiente de revisión del usuario

---

## 1. Resumen

Añadir dos mejoras al flujo de generación de headers `.h`:

1. **Auto-sanitize:** Reemplazar caracteres inválidos eliminándolos en vez de mostrar error.
2. **Persistencia:** Guardar el nombre de salida en `.NET Settings` para que se recuerde entre sesiones.

---

## 2. Contexto actual

**Archivo afectado:** `GifRGB565GUI/Form1.cs`

**Flujo actual en `btnGenerate_Click` (línea 270):**
1. Si el nombre está vacío → fallback a `n64.h`
2. Si es modo N64 y el textbox está vacío → error "Nombre requerido"
3. Valida separadores de ruta → error si los hay
4. Valida `Path.GetInvalidFileNameChars()` → error si los hay
5. Asegura extensión `.h`
6. Genera el header

**Problema:** El usuario debe corregir manualmente el nombre cada vez que introduce un carácter inválido.

---

## 3. Diseño

### 3.1 Auto-sanitize (eliminar caracteres inválidos)

**Nuevo comportamiento:**
- Cuando se detectan caracteres inválidos, se eliminan silenciosamente
- Se actualiza `txtOutName.Text` con el nombre sanitizado
- Se procede con la generación

**Método nuevo: `SanitizeFileName(string name)`**

```csharp
private static string SanitizeFileName(string name)
{
    char[] invalid = Path.GetInvalidFileNameChars();
    foreach (char c in invalid)
    {
        name = name.Replace(c.ToString(), "");
    }
    // También eliminar separadores de ruta por seguridad
    name = name.Replace(Path.DirectorySeparatorChar.ToString(), "");
    name = name.Replace(Path.AltDirectorySeparatorChar.ToString(), "");
    return name.Trim();
}
```

**Ubicación:** Dentro de la clase `Form1`, método privado.

**Integración en `btnGenerate_Click`:**

```csharp
// Reemplazar el bloque de validación actual (líneas 312-328) por:
string candidate = txtOutName.Text.Trim();
string sanitized = SanitizeFileName(candidate);

if (string.IsNullOrEmpty(sanitized))
{
    MessageBox.Show("El nombre resulta vacío tras eliminar caracteres inválidos.", 
        "Nombre inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    txtOutName.Focus();
    return;
}

if (sanitized != candidate)
{
    txtOutName.Text = sanitized;
    Log($"Nombre sanitizado: '{candidate}' → '{sanitized}'");
}
```

**Edge cases:**
- Nombre vacío tras sanitización → error
- Solo espacios → error (trim + sanitización resulta vacío)
- Nombre ya válido → sin cambios, sin log

### 3.2 Persistencia en .NET Settings

**Nueva propiedad de settings:**
- Nombre: `LastOutputName`
- Tipo: `string`
- Valor por defecto: `""`
- Alcance: `User` (por usuario)

**En `Form1_Load` (línea 37):**

```csharp
// Cargar nombre guardado
if (!string.IsNullOrEmpty(Properties.Settings.Default.LastOutputName))
{
    txtOutName.Text = Properties.Settings.Default.LastOutputName;
}
```

**En `btnGenerate_Click` (tras sanitización, antes de generar):**

```csharp
// Guardar nombre para próxima sesión
Properties.Settings.Default.LastOutputName = txtOutName.Text.Trim();
Properties.Settings.Default.Save();
```

**Alcance:** Solo se persiste para el modo N64 (headers `.h`). Las exportaciones bin no se persisten.

---

## 4. Archivos a modificar

| Archivo | Cambio |
|---------|--------|
| `GifRGB565GUI/Form1.cs` | Añadir `SanitizeFileName()`, modificar `btnGenerate_Click`, modificar `Form1_Load` |
| `GifRGB565GUI/Properties/Settings.settings` | Añadir `LastOutputName` (string, User scope) |
| `GifRGB565GUI/Properties/Settings.Designer.cs` | Se genera automáticamente al editar Settings.settings |

---

## 5. Flujo final

```
 Usuario introduce nombre
        │
        ▼
 ¿Está vacío? ──Sí──→ Error "Nombre requerido"
        │No
        ▼
 SanitizeFileName(candidate)
        │
        ▼
 ¿Resultado vacío? ──Sí──→ Error "Nombre vacío tras sanitizar"
        │No
        ▼
 ¿Cambió? ──Sí──→ Actualizar textbox + log
        │No
        ▼
 Guardar en Settings.Default.LastOutputName
        │
        ▼
 Asegurar extensión .h
        │
        ▼
 Generar header
```

---

## 6. Criterios de aceptación

- [ ] Caracteres inválidos se eliminan sin mostrar error
- [ ] `txtOutName.Text` se actualiza con el nombre sanitizado
- [ ] Se muestra en el log el cambio realizado
- [ ] Si el nombre resulta vacío, se muestra error apropiado
- [ ] El nombre se guarda al generar y se carga al abrir la aplicación
- [ ] No se persiste nada para exportaciones bin
- [ ] La extensión `.h` se sigue asegurando correctamente

---

## 7. No incluido

- Undo/rollback del sanitizado
- Persistencia de opciones de dithering/noise/sharpen
- Persistencia del formato de exportación
- Sanitize en tiempo real (mientras el usuario escribe)
