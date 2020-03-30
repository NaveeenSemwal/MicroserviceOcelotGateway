
#https://medium.com/swlh/containerize-asp-net-core-3-1-with-docker-c5e1acabba21
#https://docs.microsoft.com/en-us/virtualization/windowscontainers/quick-start/building-sample-app



FROM mcr.microsoft.com/dotnet/core/sdk:3.1-nanoserver-1903 AS build-env
WORKDIR /app

COPY  TweetBook/TweetBook.csproj ./
RUN dotnet restore


COPY . ./
RUN dotnet publish TweetBook/TweetBook.csproj -c Release -o /app/publish



FROM mcr.microsoft.com/dotnet/core/sdk:3.1-nanoserver-1903
WORKDIR /app
EXPOSE 80
COPY --from=build-env /app/publish/ .
ENTRYPOINT ["dotnet", "TweetBook.dll"]