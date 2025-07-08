# --- Etapa 1: Build (Construcción) ---
# Usamos la imagen del SDK de .NET para construir el proyecto.
# Esta imagen contiene todas las herramientas necesarias para compilar y publicar la aplicación.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copia los archivos de proyecto (.csproj) primero para aprovechar el caché de capas de Docker.
# Si estos archivos no cambian, Docker no necesitará restaurar las dependencias de nuevo.
COPY *.csproj ./
RUN dotnet restore

# Copia el resto del código fuente de la aplicación.
COPY . ./

# Publica la aplicación en modo Release, optimizada para producción.
# La salida se colocará en el directorio /app/out.
RUN dotnet publish -c Release -o out

# --- Etapa 2: Runtime (Ejecución) ---
# Usamos la imagen de ASP.NET runtime, que es mucho más ligera que la del SDK.
# Esta imagen solo contiene lo necesario para ejecutar la aplicación, no para compilarla.
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Expone el puerto 8080, que es donde la aplicación escuchará las peticiones.
# Puedes cambiarlo al puerto que tu aplicación utilice.
EXPOSE 8080

# Copia la salida de la compilación desde la etapa anterior (build-env) a la imagen final.
COPY --from=build-env /app/out .

# Define el comando que se ejecutará cuando el contenedor inicie.
# Esto inicia la aplicación. El nombre del archivo .dll debe coincidir con el de tu proyecto.
ENTRYPOINT ["dotnet", "ProductsService.dll"]
