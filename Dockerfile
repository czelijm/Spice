#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Spice/Spice.csproj", "Spice/"]
RUN dotnet restore "Spice/Spice.csproj"
COPY . .
WORKDIR "/src/Spice"
RUN dotnet build "Spice.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Spice.csproj" -c Release -o /app/publish
RUN dotnet dev-certs https --clean
# RUN dotnet dev-certs https --trust

FROM base AS final
#important for heroku!!!!
#ENV ASPNETCORE_URLS="http://*:$PORT"
#ENV ASPNETCORE_ENVIRONMENT="Development"
ENV STRIPE_SECRETKEY="sk_test_51IC9HWE0ekSlhYUhlEmAm46ozFPbzIFGl0cA7i0T1NSyLxPPgiA4uoxb911djYNcHXsC8qBHADcix3pxGUkMnFE300g9plBgxw"
ENV STRIPE_PUBLISHABLEKEY="pk_test_51IC9HWE0ekSlhYUhyZ0Pfx5Hv83FlKuxOCPV1JmxJ51Ha5P1D9EtUndVoQHfQpWVoCCE6TX6bPiwVa403HwLSTj600oLT0opXs"
ENV FACEBOOK_APPID="527888031497313"
ENV FACEBOOK_APPSECRET="5349e5f7f6535aa1072f6e2ba48399aa"
ENV SENDGRIP_APIKEY="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"
ENV SENDGRIP_EMAILCOMPANY="marekczelij1@gmail.com"
ENV ADMINACCOUNTINFO_USERNAME="Admin@Spice.com"
ENV ADMINACCOUNTINFO_EMAIL="marekczelij1@gmail.com"
ENV ADMINACCOUNTINFO_EMAILCONFIRMED="true"
ENV ADMINACCOUNTINFO_NAME="Admin Citizen"
ENV ADMINACCOUNTINFO_PHONE="7777777778"
ENV ADMINACCOUNTINFO_PASSWORD="<VEry.STRong.PASSword101!XD>"
ENV DATABASE_URL="postgres://nzrwsdkphoqvzw:e33739d837f430667d23045f9666ed163a96f9b5e1ed409c9353511f85ccf61e@ec2-52-4-171-132.compute-1.amazonaws.com:5432/d1v8o28to249cu"
WORKDIR /app
COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "Spice.dll"]
#important for heroku!!!!
CMD ASPNETCORE_URLS="http://*:$PORT" dotnet Spice.dll 
