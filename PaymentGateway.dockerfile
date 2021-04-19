
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

COPY . ./
RUN dotnet restore
RUN dotnet publish PaymentGateway/PaymentGateway.csproj -c Release -o out

RUN mkdir -p ./out/Utils
COPY PaymentGateway/Utils/TableScripts.sql ./out/Utils

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "PaymentGateway.dll"]