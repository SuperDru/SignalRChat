FROM microsoft/dotnet:2.2-sdk

WORKDIR /source

COPY . .
RUN dotnet publish ./WebChat/WebChat.csproj --output /app

ENTRYPOINT ["dotnet", "/app/WebChat.dll"]