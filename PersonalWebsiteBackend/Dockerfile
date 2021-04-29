# credits to https://stackoverflow.com/a/57440303

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

COPY . ./
RUN dotnet publish PersonalWebsiteBackend -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/out .

# presence of appsettings.json with valid (e.g. empty json) content is needed for dotnet PersonalWebsiteBackend.dll
# the current appsettings.json is in .dockerignore to not copy secrets into image
RUN echo "{}" > appsettings.json

# CMD /bin/bash ; sleep infinity

ENTRYPOINT ["dotnet", "PersonalWebsiteBackend.dll"]
