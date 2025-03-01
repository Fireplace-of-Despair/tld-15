FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /app

WORKDIR /src
COPY ["TLD15/TLD15.csproj", "./TLD15/"]
COPY ["ACherryPie/ACherryPie.csproj", "./ACherryPie/"]
COPY ["Common/Common.csproj", "./Common/"]
COPY ["Infrastructure/Infrastructure.csproj", "./Infrastructure/"]

RUN dotnet restore "TLD15/TLD15.csproj"

COPY . .

RUN dotnet publish "TLD15/TLD15.csproj" -c Release -o /app/out

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime

# Enable globalization and time zones:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/enable-globalization.md
ENV \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    LC_ALL=en_US.UTF-8 \
    LANG=en_US.UTF-8

RUN apk add --no-cache \
    icu-data-full \
    icu-libs

ENV TZ UTC
RUN apk --no-cache add tzdata

WORKDIR /app
COPY --from=build /app/out ./

# Expose port 8080 for the web application
EXPOSE 8080

# Set the entry point for the container
ENTRYPOINT ["dotnet", "./TLD15.dll"]
