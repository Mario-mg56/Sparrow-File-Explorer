# 📌 Arquitectura MVVM

## 🔹 ¿Qué es MVVM y para qué sirve?

MVVM (Model–View–ViewModel) es un patrón de arquitectura que permite **separar la lógica de la interfaz de usuario de la lógica de la aplicación**.

### ✔ Objetivos principales

- Mejorar la **mantenibilidad**
- Facilitar la **testabilidad**
- Permitir **reutilización de código**
- Escalar el proyecto de forma ordenada

---

# 🧠 Componentes del patrón

---

## 🔹 Views (Vista)

Es la **interfaz gráfica** del usuario.

### ✔ Qué incluye

- Archivos `.axaml` (en Avalonia)
- Botones, listas, layouts
- Enlaces de datos (bindings)

### ❌ Qué NO incluye

- Lógica de negocio
- Acceso a datos
- Decisiones del sistema

### 💡 Cómo identificarla

> “Esto se muestra en pantalla”

---

## 🔹 ViewModels

Es el **intermediario entre la vista y el modelo**.

### ✔ Qué incluye

- Propiedades visibles en la UI
- Comandos (acciones del usuario)
- Lógica de presentación
- Implementación de `INotifyPropertyChanged`

### ❌ Qué NO incluye

- Acceso directo a la UI
- Lógica de negocio compleja
- Dependencias de la vista

### 💡 Cómo identificarlo

> “Esto responde a acciones del usuario y alimenta la UI”

---

# ⚙️ Estructura del repositorio

## 📁 /Models (Dominio)

Contiene la **representación del sistema y sus reglas**.

### ✔ Incluye

- Entidades (`FileItem`, `DirectoryItem`, etc.)
- Validaciones
- Lógica de negocio pura

### ❌ No incluye

- Acceso a archivos
- Llamadas externas
- UI

---


## 📁 /Infrastructures (externo)

Contiene todo lo que conecta la aplicación con el exterior.

### ✔ Incluye

- Acceso al sistema de archivos (`System.IO`)
- Bases de datos
- APIs externas
- Servicios técnicos (logging, cache)

### ❌ No incluye

- Lógica de negocio
- Decisiones del sistema

---

## 📁 /Services (casos de uso)

Contiene la **lógica de aplicación** (qué se hace y en qué orden).

### ✔ Incluye

- Casos de uso:
  - Listar carpeta
  - Abrir archivo
  - Eliminar archivo
- Coordinación entre Model e Infrastructure

### ❌ No incluye

- UI
- Detalles técnicos de bajo nivel

---

## 📁 /Helpers (utilidades)

Contiene funciones auxiliares reutilizables.

### ✔ Incluye

- Conversores de datos
- Operaciones con strings, rutas, fechas
- Métodos genéricos

### ❌ No incluye

- Lógica de negocio
- Estado del sistema

---

# 🔄 Flujo de ejecución


### ✔ Paso a paso

1. El usuario interactúa con la **View**
2. La View envía la acción al **ViewModel**
3. El ViewModel llama a un **Service**
4. El Service usa la **Infrastructure** (ej: sistema de archivos)
5. Se crean/usan **Models**
6. Los datos vuelven al ViewModel
7. La View se actualiza mediante binding

---

# 🎯 Ejemplo mental

> El usuario hace clic en una carpeta:

- View → detecta el click  
- ViewModel → ejecuta comando  
- Service → pide listar la carpeta  
- Infrastructure → accede al sistema de archivos  
- Model → representa los archivos  
- ViewModel → actualiza la lista  
- View → se refresca automáticamente  

---

# ✅ Resumen final

MVVM separa la aplicación en:

- **Model** → datos y reglas  
- **View** → interfaz  
- **ViewModel** → lógica de presentación  

Y se apoya en:

- **Services** → lógica de aplicación  
- **Infrastructure** → acceso a recursos externos  
- **Helpers** → utilidades  

👉 Resultado: código organizado, mantenible y escalable.
