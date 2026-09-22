# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# copy csproj and restore
COPY ["Snake/Snake.csproj", "Snake/"]
RUN dotnet restore "Snake/Snake.csproj"

# copy everything and publish
COPY . .
WORKDIR /src/Snake
RUN dotnet publish "Snake.csproj" -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /app/publish .

# When running the container, you probably want to allocate a TTY and interactive mode:
# docker run -it --rm <image>
ENTRYPOINT ["dotnet", "Snake.dll"]
