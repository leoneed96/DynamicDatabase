FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

WORKDIR /app
COPY *.sln .
COPY ConsoleApp1/*.csproj ./ConsoleApp1/
RUN dotnet restore --ignore-failed-sources -s https://www.nuget.org/api/v2/ --source https://nexus.npc.ba/repository/nuget-all/

COPY ConsoleApp1/. ./ConsoleApp1/
WORKDIR /app/ConsoleApp1
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime


RUN apt-get -y update \
    && apt-get install -y libc6-dev libgdiplus libc6 fontconfig libharfbuzz0b libfreetype6 libosmesa6 libglu1-mesa \
    && apt-get -y clean

WORKDIR /app
COPY --from=build /app/ConsoleApp1/out ./

ENTRYPOINT [ "dotnet", "ConsoleApp1.dll" ]
