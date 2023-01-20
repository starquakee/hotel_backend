# docker build -t ccr.ccs.tencentyun.com/hotel_management/backend -f HotelManagement/Dockerfile .

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["HotelManagement/HotelManagement.csproj", "HotelManagement/"]
RUN dotnet restore "HotelManagement/HotelManagement.csproj"
COPY . .
WORKDIR "/src/HotelManagement"
RUN dotnet build "HotelManagement.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HotelManagement.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HotelManagement.dll"]