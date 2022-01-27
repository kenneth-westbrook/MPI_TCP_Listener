#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["VBS.MPI_TCP_Listener/VBS.MPI_TCP_Listener.csproj", "VBS.MPI_TCP_Listener/"]
RUN dotnet restore "VBS.MPI_TCP_Listener/VBS.MPI_TCP_Listener.csproj"
COPY . .
WORKDIR "/src/VBS.MPI_TCP_Listener"
RUN dotnet build "VBS.MPI_TCP_Listener.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VBS.MPI_TCP_Listener.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VBS.MPI_TCP_Listener.dll"]