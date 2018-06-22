FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS http://+:8086

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY snsEnpoint/snsEnpoint.csproj snsEnpoint/
RUN dotnet restore snsEnpoint/snsEnpoint.csproj
COPY . .
WORKDIR /src/snsEnpoint
RUN dotnet build snsEnpoint.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish snsEnpoint.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "snsEnpoint.dll"]
