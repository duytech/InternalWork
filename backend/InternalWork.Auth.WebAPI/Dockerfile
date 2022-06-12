#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["InternalWork.Auth.WebAPI/InternalWork.Auth.csproj", "InternalWork.Auth.WebAPI/"]
COPY ["InternalWork.Auth.Services/InternalWork.Auth.Services.csproj", "InternalWork.Auth.Services/"]
COPY ["InternalWork.Auth.Data/InternalWork.Auth.Data.csproj", "InternalWork.Auth.Data/"]
COPY ["InternalWork.Auth.Common/InternalWork.Auth.Common.csproj", "InternalWork.Auth.Common/"]
RUN dotnet restore "InternalWork.Auth.WebAPI/InternalWork.Auth.csproj"
COPY . .
WORKDIR "/src/InternalWork.Auth.WebAPI"
RUN dotnet build "InternalWork.Auth.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InternalWork.Auth.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InternalWork.Auth.dll"]