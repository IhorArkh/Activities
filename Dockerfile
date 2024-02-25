FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app
EXPOSE 8080

#cope .csproj and restore to distinct layers
COPY "Reactivities/Reactivities.sln" "Reactivities/Reactivities.sln"
COPY "Reactivities/API/API.csproj" "Reactivities/API/API.csproj"
COPY "Reactivities/Application/Application.csproj" "Reactivities/Application/Application.csproj"
COPY "Reactivities/Persistence/Persistence.csproj" "Reactivities/Persistence/Persistence.csproj"
COPY "Reactivities/Domain/Domain.csproj" "Reactivities/Domain/Domain.csproj"
COPY "Reactivities/Infrastructure/Infrastructure.csproj" "Reactivities/Infrastructure/Infrastructure.csproj"

RUN ls -la
RUN dotnet restore "Reactivities/Reactivities.sln"

#copy everything else build
COPY . .
WORKDIR /app
RUN ls -la
RUN dotnet publish -c Release -o out "Reactivities/Reactivities.sln"

#build a runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .
RUN ls -la
ENTRYPOINT ["dotnet", "API.dll"]