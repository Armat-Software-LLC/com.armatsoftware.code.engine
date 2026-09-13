FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore ArmatSoftware.Code.Engine.Tester.WebApi/ArmatSoftware.Code.Engine.Tester.WebApi.csproj
RUN dotnet publish ArmatSoftware.Code.Engine.Tester.WebApi/ArmatSoftware.Code.Engine.Tester.WebApi.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ArmatSoftware.Code.Engine.Tester.WebApi.dll"]
