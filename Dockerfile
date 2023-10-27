FROM mcr.microsoft.com/dotnet/sdk:7.0.403 AS build-env

WORKDIR /src

# copy csproj and restore as distinct layers
COPY ./src/ValMati.StockBot/ValMati.StockBot.csproj ./src/ValMati.StockBot/
RUN dotnet restore ./src/ValMati.StockBot/ValMati.StockBot.csproj --runtime linux-x64

# copy everything else and build
COPY . .
RUN dotnet publish ./src/ValMati.StockBot/ValMati.StockBot.csproj -c Release -o /publish --no-restore --runtime linux-x64 --no-self-contained /p:DebugType=None /p:Platform=x64

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0.13
WORKDIR /app

COPY --from=build-env /publish .

ENTRYPOINT ["dotnet", "ValMati.StockBot.dll"]
