#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ActivationServer/QRSharingApp.ActivationWebApp/QRSharingApp.ActivationWebApp.csproj", "ActivationServer/QRSharingApp.ActivationWebApp/"]
RUN dotnet restore "ActivationServer/QRSharingApp.ActivationWebApp/QRSharingApp.ActivationWebApp.csproj"
COPY . .
WORKDIR "/src/ActivationServer/QRSharingApp.ActivationWebApp"
RUN dotnet build "QRSharingApp.ActivationWebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "QRSharingApp.ActivationWebApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QRSharingApp.ActivationWebApp.dll"]