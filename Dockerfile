# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the solution file
COPY Redot-Documentation.sln .

# Copy the main project file and restore dependencies
COPY Redot-Documentation/Redot-Documentation.csproj ./Redot-Documentation/
RUN dotnet restore "Redot-Documentation/Redot-Documentation.csproj"

# Copy the client project file (if it exists, as implied by the folder structure)
COPY Redot-Documentation.Client/Redot-Documentation.Client.csproj ./Redot-Documentation.Client/ 2>/dev/null || true
# The line above uses a trick to ignore error if the client project doesn't exist

# Copy all source code
COPY . .

# Build and publish the main project
WORKDIR /src/Redot-Documentation
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080

# Copy the published output
COPY --from=build /app/publish .

# Set environment variable for the port
ENV ASPNETCORE_URLS=http://+:8080

# Run the application
ENTRYPOINT ["dotnet", "Redot-Documentation.dll"]