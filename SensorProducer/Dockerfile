FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["SensorProducer.csproj", "./"]
RUN dotnet restore "SensorProducer.csproj"

COPY . .
RUN dotnet publish "SensorProducer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "SensorProducer.dll"]