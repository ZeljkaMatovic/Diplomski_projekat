FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY BackEnd.csproj .
RUN dotnet restore  --verbosity n
COPY . .
RUN dotnet publish  --verbosity n -c release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "BackEnd.dll"]