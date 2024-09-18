ARG DOTNET_SDK_TAG
ARG MONITOR_TAG
FROM mcr.microsoft.com/dotnet/sdk:$DOTNET_SDK_TAG as restore
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_CLI_UI_LANGUAGE=en-US \
    DOTNET_SVCUTIL_TELEMETRY_OPTOUT=1 \
    DOTNET_NOLOGO=1 \
    POWERSHELL_TELEMETRY_OPTOUT=1 \
    POWERSHELL_UPDATECHECK_OPTOUT=1 \
    DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1 \
# workaround of https://github.com/microsoft/playwright-dotnet/issues/1791
    DOTNET_ROLL_FORWARD=Major \
    DOTNET_ROLL_FORWARD_TO_PRERELEASE=1 \
    NUGET_CERT_REVOCATION_MODE=offline

WORKDIR /src
COPY *.sln ./
COPY */*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done
RUN dotnet restore


FROM restore as build
COPY . .
RUN dotnet publish --no-restore --nologo -c Release -nodeReuse:false -o /app Monitoring.Extension.GoogleCloudStorage/Monitoring.Extension.GoogleCloudStorage.csproj

FROM mcr.microsoft.com/dotnet/monitor/base:$MONITOR_TAG
COPY --from=build ["/app", "/app/extensions/GoogleCloudStorage"]