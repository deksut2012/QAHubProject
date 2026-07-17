FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["apps/api/QAHub.Api.csproj", "apps/api/"]
RUN dotnet restore "apps/api/QAHub.Api.csproj"
COPY apps/api/ apps/api/
RUN dotnet publish "apps/api/QAHub.Api.csproj" -c Release -o /app --no-restore -p:NoWarn=CA1861

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app .
USER $APP_UID
ENTRYPOINT ["dotnet", "QAHub.Api.dll"]
