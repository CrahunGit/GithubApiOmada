#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["GithubApiOmada/GithubApiOmada.csproj", "GithubApiOmada/"]
RUN dotnet restore "GithubApiOmada/GithubApiOmada.csproj"
COPY . .
WORKDIR "/src/GithubApiOmada"
RUN dotnet build "GithubApiOmada.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GithubApiOmada.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GithubApiOmada.dll"]