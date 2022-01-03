# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/aspnet:6.0
COPY bin/Release/net6.0/ IkeaNotifier/
WORKDIR /IkeaNotifier
EXPOSE 5000
ENV ASPNETCORE_URLS http://*:5000
ENTRYPOINT ["dotnet", "IkeaNotifier.dll",  "--server.urls", "https://+:5000"]