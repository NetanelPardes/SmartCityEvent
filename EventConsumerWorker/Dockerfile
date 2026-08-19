FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["EventConsumerWorker.csproj", "./"]
RUN dotnet restore "EventConsumerWorker.csproj"

COPY . .
RUN dotnet publish "EventConsumerWorker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "EventConsumerWorker.dll"]