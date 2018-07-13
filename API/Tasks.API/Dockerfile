FROM microsoft/aspnetcore:2.0-nanoserver-1709 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0-nanoserver-1709 AS build
WORKDIR /src
COPY Tasks.API/Tasks.API.csproj Tasks.API/
RUN dotnet restore Tasks.API/Tasks.API.csproj
COPY . .
WORKDIR /src/Tasks.API
RUN dotnet build Tasks.API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Tasks.API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Tasks.API.dll"]
