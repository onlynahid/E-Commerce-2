FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["AYYUAZ.APP/AYYUAZ.APP.csproj", "AYYUAZ.APP/"]
COPY ["AYYUAZ.APP.Application/AYYUAZ.APP.Application.csproj", "AYYUAZ.APP.Application/"]
COPY ["AYYUAZ.APP.Infrastructure/AYYUAZ.APP.Infrastructure.csproj", "AYYUAZ.APP.Infrastructure/"]
COPY ["AYYUAZ.APP.Domain/AYYUAZ.APP.Domain.csproj", "AYYUAZ.APP.Domain/"]

RUN dotnet restore "AYYUAZ.APP/AYYUAZ.APP.csproj"

COPY . .
RUN dotnet publish "AYYUAZ.APP/AYYUAZ.APP.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://0.0.0.0:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "AYYUAZ.APP.dll"]
