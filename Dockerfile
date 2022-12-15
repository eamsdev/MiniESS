FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Install Node.js
RUN curl -fsSL https://deb.nodesource.com/setup_16.x | bash - \
    && apt-get install -y \
        nodejs \
    && rm -rf /var/lib/apt/lists/*
	
RUN npm install -g yarn

# Copy csproj files
WORKDIR /src
COPY ["MiniESS.Todo/MiniESS.Todo.csproj", "MiniESS.Todo/"]
COPY ["MiniESS.Core/MiniESS.Core.csproj", "MiniESS.Core/"]
COPY ["MiniESS.Infrastructure/MiniESS.Infrastructure.csproj", "MiniESS.Infrastructure/"]

# restore dependencies
RUN dotnet restore "MiniESS.Todo/MiniESS.Todo.csproj"
RUN dotnet restore "MiniESS.Core/MiniESS.Core.csproj"
RUN dotnet restore "MiniESS.Infrastructure/MiniESS.Infrastructure.csproj"
COPY . .
WORKDIR "/src/MiniESS.Todo"
RUN dotnet build "MiniESS.Todo.csproj" -c Release

FROM build AS publish
RUN dotnet publish "MiniESS.Todo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MiniESS.Todo.dll"]
