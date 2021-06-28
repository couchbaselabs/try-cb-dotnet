FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build-env

LABEL maintainer="Couchbase"

WORKDIR /app

ADD . /app 

ENV ASPNETCORE_URLS=http://+:8080 

# Expose ports
EXPOSE 8080

# Set the entrypoint 
ENTRYPOINT ["dotnet", "run", "--project", "try-cb-dotnet/try-cb-dotnet.csproj"]
