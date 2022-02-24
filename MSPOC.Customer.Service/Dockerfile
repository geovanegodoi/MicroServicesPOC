FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 5001

ENV ASPNETCORE_URLS=http://+:5001

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["./src/MSPOC.Customer.Service.csproj", "./"]
COPY ["./src/packages/*.nupkg", "./packages/"]
COPY ["./nuget.config", "./"]
RUN dotnet restore "./MSPOC.Customer.Service.csproj"
COPY . .
RUN dotnet build "./MSPOC.Customer.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./MSPOC.Customer.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY ["./src/dotnet.css", "./"]
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MSPOC.Customer.Service.dll"]

# docker build -t ggodoi1986/mspoc.customer.service:1.0.0 .