#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Backend.Processor/Backend.Processor.csproj", "Backend.Processor/"]
RUN dotnet restore "Backend.Processor/Backend.Processor.csproj"
COPY . .
WORKDIR "/src/Backend.Processor"
RUN dotnet build "Backend.Processor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Backend.Processor.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Backend.Processor.dll"]