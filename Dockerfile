# ===================================================
# المرحلة 1: البناء (Build Stage)
# ===================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# نسخ ملفات الـ .csproj أولاً لاستخدام Docker cache بذكاء
COPY Donations.sln .
COPY Donations/Donations.csproj Donations/
COPY Donation.Applecation/Donation.Application.csproj Donation.Applecation/
COPY Donation.Domain/Donation.Domain.csproj Donation.Domain/
COPY Donation.Infrastructure/Donation.Infrastructure.csproj Donation.Infrastructure/
COPY BuildingBlocks.Logging/BuildingBlocks.Logging.csproj BuildingBlocks.Logging/
COPY BuildingBlocks.Shared.Contracts/BuildingBlocks.Shared.Contracts.csproj BuildingBlocks.Shared.Contracts/
COPY BuildingBlocks.Shared.Core/BuildingBlocks.Shared.Core.csproj BuildingBlocks.Shared.Core/

RUN dotnet restore

# نسخ باقي الكود وبناء المشروع
COPY . .
RUN dotnet publish Donations/Donations.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ===================================================
# المرحلة 2: التشغيل (Runtime Stage)
# ===================================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Donations.dll"]
