# credits to https://stackoverflow.com/a/57440303

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

COPY . ./
RUN dotnet publish PersonalWebsiteBackend -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/out .

# ENTRYPOINT ["dotnet", "PersonalWebsiteBackend.dll"]