# Use the official .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the solution and project files
COPY ["Scoring Service.sln", "./"]
COPY ["Scoring Service/", "Scoring Service/"]

# Restore dependencies
WORKDIR /app/Scoring Service
RUN dotnet restore

# Build the application
RUN dotnet publish -c Release -o /out

# Use the runtime image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy the published application
COPY --from=build /out .

# Expose the port your application runs on
EXPOSE 5001

# Run the application
ENTRYPOINT ["dotnet", "Scoring Service.dll"]
