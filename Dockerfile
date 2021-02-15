#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Spice/Spice.csproj", "Spice/"]
RUN dotnet restore "Spice/Spice.csproj"
COPY . .
WORKDIR "/src/Spice"
RUN dotnet build "Spice.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Spice.csproj" -c Release -o /app/publish
RUN dotnet dev-certs https --clean
# RUN dotnet dev-certs https --trust

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Spice.dll"]