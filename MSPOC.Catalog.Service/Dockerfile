FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["./src/MSPOC.Catalog.Service.csproj", "./"]
COPY ["./packages/", "./packages/"]
COPY ["./nuget.config", "./"]
RUN dotnet restore "./MSPOC.Catalog.Service.csproj"
COPY . .
RUN dotnet build "./MSPOC.Catalog.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./MSPOC.Catalog.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY ["./src/dotnet.css", "./"]
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MSPOC.Catalog.Service.dll"]

# docker build -t ggodoi1986/mspoc.catalog.service:1.0.0 .