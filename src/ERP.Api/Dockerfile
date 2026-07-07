FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
ENV DOTNET_SYSTEM_NET_HTTP_SOCKETSHTTPHANDLER_HTTP2SUPPORT=false

COPY ERP-Intelligence.sln ./
COPY src/ERP.Domain/ERP.Domain.csproj src/ERP.Domain/
COPY src/ERP.SharedKernel/ERP.SharedKernel.csproj src/ERP.SharedKernel/
COPY src/ERP.Application/ERP.Application.csproj src/ERP.Application/
COPY src/ERP.Infrastructure/ERP.Infrastructure.csproj src/ERP.Infrastructure/
COPY src/ERP.Api/ERP.Api.csproj src/ERP.Api/

RUN dotnet restore src/ERP.Api/ERP.Api.csproj --disable-parallel

COPY src/ src/
RUN dotnet publish src/ERP.Api/ERP.Api.csproj --configuration Release --no-restore --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ERP.Api.dll"]
