FROM mcr.microsoft.com/dotnet/aspnet:7.0.10-alpine3.18-amd64 AS base
WORKDIR /app
EXPOSE 443  
EXPOSE 80  
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Pb.ApiGateway/Pb.ApiGateway.csproj", "src/Pb.ApiGateway/"]
COPY ["src/Pb.ApiGateway/Pb.Geo.csproj", "src/Pb.ApiGateway/"]
COPY ["src/Pb.ApiGateway/Pb.Profile.Service.csproj", "src/Pb.ApiGateway/"]
COPY ["src/Pb.ApiGateway/Pb.Search.Service.csproj", "src/Pb.ApiGateway/"]
COPY ["src/Pb.ApiGateway/Pb.Rate.Service.csproj", "src/Pb.ApiGateway/"]
COPY ["src/Pb.Common", "src/Pb.Common"]
COPY . .

RUN apt-get update && apt-get install -y unzip

RUN unzip src/Pb.ApiGateway.Service/Data/inventory.zip -d src/Pb.ApiGateway.Service/Data
RUN unzip src/Pb.ApiGateway.Service/Data/geo.zip -d src/Pb.ApiGateway.Service/Data
RUN unzip src/Pb.ApiGateway.Service/Data/hotels.zip -d src/Pb.ApiGateway.Service/Data

RUN dotnet restore "src/Pb.ApiGateway/Pb.Geo.Service.csproj"

WORKDIR "/src/src/Pb.ApiGateway"
RUN dotnet build "Pb.ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pb.ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pb.ApiGateway.dll"]
