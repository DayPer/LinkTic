# InventoryService/Dockerfile
# Etapa de construcción (Build Stage)
# Utiliza la imagen del SDK de .NET 8.0 para compilar la aplicación.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Establece el directorio de trabajo dentro del contenedor.
# Todos los comandos subsiguientes se ejecutarán desde aquí.
WORKDIR /src

# Copia el archivo .csproj al directorio de trabajo.
# La fuente "InventoryService.csproj" es relativa al contexto de construcción
# (que es la carpeta 'InventoryService' en tu máquina, según docker-compose.yml).
# El destino "./" significa el directorio actual dentro del contenedor (/src).
COPY ["InventoryService.csproj", "./"]

# Restaura las dependencias del proyecto.
# La ruta "InventoryService.csproj" es relativa al WORKDIR actual (/src).
RUN dotnet restore "InventoryService.csproj"

# Copia el resto de los archivos del proyecto al directorio de trabajo.
# El primer "." se refiere al contexto de construcción (la carpeta 'InventoryService').
# El segundo "." se refiere al WORKDIR actual dentro del contenedor (/src).
COPY . .

# Cambia el directorio de trabajo al subdirectorio del proyecto dentro de /src.
# Esto es esencial para que 'dotnet build' y 'dotnet publish' encuentren el proyecto
# correctamente si tu estructura de solución .NET es más compleja.
# En este caso, si tu .csproj está directamente en /src, puedes mantener WORKDIR /src.
# Si tu proyecto estuviera en /src/InventoryService (ej. si la solución fuera la raíz),
# entonces necesitarías WORKDIR "/src/InventoryService".
# Para tu caso actual, donde el .csproj se copia a /src, mantenemos WORKDIR en /src.
# WORKDIR "/src/InventoryService" # <-- Esta línea se elimina o se ajusta si tu .csproj no está en la raíz de /src

# Construye la aplicación en modo Release y la publica en /app/build.
# La ruta "InventoryService.csproj" es relativa al WORKDIR actual (/src).
RUN dotnet build "InventoryService.csproj" -c Release -o /app/build

# Etapa final (Final Stage)
# Utiliza la imagen de ASP.NET 8.0 para ejecutar la aplicación (más ligera).
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Establece el directorio de trabajo final como /app.
WORKDIR /app

# Copia los archivos publicados desde la etapa de construcción (build) a la etapa final.
COPY --from=build /app/build .

# Define el punto de entrada para ejecutar la aplicación cuando el contenedor se inicie.
ENTRYPOINT ["dotnet", "InventoryService.dll"]
