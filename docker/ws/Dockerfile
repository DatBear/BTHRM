FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG ENV_FILE

WORKDIR /src

COPY bthrm.server/bthrm.server.csproj .
RUN dotnet restore "bthrm.server.csproj"

COPY ./bthrm.core ./
COPY ./bthrm.server ./
COPY ./bthrm.server/${ENV_FILE} ./.env

RUN dotnet build "bthrm.server.csproj" -c Release -o /app/build

FROM build as publish
RUN dotnet publish "bthrm.server.csproj" -c Release -o /app/publish

FROM base as final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /src/.env .

ENTRYPOINT ["dotnet", "bthrm.server.dll"]