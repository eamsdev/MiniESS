FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Install Node.js
RUN curl -fsSL https://deb.nodesource.com/setup_14.x | bash - \
    && apt-get install -y \
        nodejs \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /src
COPY ["MiniESS.Todo/MiniESS.Todo.csproj", "MiniESS.Todo/"]
RUN dotnet restore "MiniESS.Todo/MiniESS.Todo.csproj"
COPY . .
WORKDIR "/src/MiniESS.Todo"
RUN dotnet build "MiniESS.Todo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MiniESS.Todo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MiniESS.Todo.dll"]
