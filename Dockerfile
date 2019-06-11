FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 3579

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["NerdBotCore/NerdBotCommon/NerdBotCommon.csproj", "NerdBotCore/NerdBotCommon/"]
COPY ["NerdBotCore/NerdBotHost/NerdBotHost.csproj", "NerdBotCore/NerdBotHost/"]

# Plugins
COPY ["NerdBotCore/NerdBotCorePlugin/NerdBotCorePlugin.csproj", "NerdBotCore/NerdBotCorePlugin/"]
COPY ["NerdBotCore/NerdBotScryFallPlugin/NerdBotScryFallPlugin.csproj", "NerdBotCore/NerdBotScryFallPlugin/"]
COPY ["NerdBotCore/NerdBotRoastMePlugin/NerdBotRoastMePlugin.csproj", "NerdBotCore/NerdBotRoastMePlugin/"]
COPY ["NerdBotCore/NerdBotGiphyPlugin/NerdBotGiphyPlugin.csproj", "NerdBotCore/NerdBotGiphyPlugin/"]
COPY ["NerdBotCore/NerdBotUrbanDictPlugin/NerdBotUrbanDictPlugin.csproj", "NerdBotCore/NerdBotUrbanDictPlugin/"]

RUN dotnet restore "NerdBotCore/NerdBotCommon/NerdBotCommon.csproj"
RUN dotnet restore "NerdBotCore/NerdBotHost/NerdBotHost.csproj"

# Plugins
RUN dotnet restore "NerdBotCore/NerdBotCorePlugin/NerdBotCorePlugin.csproj"
RUN dotnet restore "NerdBotCore/NerdBotScryFallPlugin/NerdBotScryFallPlugin.csproj"
RUN dotnet restore "NerdBotCore/NerdBotRoastMePlugin/NerdBotRoastMePlugin.csproj"
RUN dotnet restore "NerdBotCore/NerdBotGiphyPlugin/NerdBotGiphyPlugin.csproj"
RUN dotnet restore "NerdBotCore/NerdBotUrbanDictPlugin/NerdBotUrbanDictPlugin.csproj"

COPY . .
WORKDIR "/src/NerdBotCore/NerdBotCommon"
RUN dotnet build "NerdBotCommon.csproj" -c Release -o /app

# Build plugins
WORKDIR "/src/NerdBotCore/NerdBotCorePlugin"
RUN dotnet build "NerdBotCorePlugin.csproj" -c Release -o /app/plugins

WORKDIR "/src/NerdBotCore/NerdBotScryFallPlugin"
RUN dotnet build "NerdBotScryFallPlugin.csproj" -c Release -o /app/plugins

WORKDIR "/src/NerdBotCore/NerdBotRoastMePlugin"
RUN dotnet build "NerdBotRoastMePlugin.csproj" -c Release -o /app/plugins

WORKDIR "/src/NerdBotCore/NerdBotGiphyPlugin"
RUN dotnet build "NerdBotGiphyPlugin.csproj" -c Release -o /app/plugins

WORKDIR "/src/NerdBotCore/NerdBotUrbanDictPlugin"
RUN dotnet build "NerdBotUrbanDictPlugin.csproj" -c Release -o /app/plugins

WORKDIR "/src/NerdBotCore/NerdBotHost"
RUN dotnet build "NerdBotHost.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "NerdBotHost.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "NerdBotHost.dll"]