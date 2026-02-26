FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["OphthalmicRegistry.API/OphthalmicRegistry.API.csproj", "OphthalmicRegistry.API/"]
COPY ["OphthalmicRegistry.Application/OphthalmicRegistry.Application.csproj", "OphthalmicRegistry.Application/"]
COPY ["OphthalmicRegistry.Domain/OphthalmicRegistry.Domain.csproj", "OphthalmicRegistry.Domain/"]
COPY ["OphthalmicRegistry.Infrastructure/OphthalmicRegistry.Infrastructure.csproj", "OphthalmicRegistry.Infrastructure/"]
RUN dotnet restore "OphthalmicRegistry.API/OphthalmicRegistry.API.csproj"
COPY . .
WORKDIR "/src/OphthalmicRegistry.API"
RUN dotnet build "OphthalmicRegistry.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OphthalmicRegistry.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OphthalmicRegistry.API.dll"]
