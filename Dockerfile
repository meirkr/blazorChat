FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as builder

WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./

# Build runtime image
RUN dotnet publish -c Release -o out


FROM nginx:alpine as final
## Copy our default nginx config
COPY nginx.conf /etc/nginx/nginx.conf

## Remove default nginx website
# RUN rm -rf /usr/share/nginx/html/*

WORKDIR /usr/share/nginx/html

## From ‘builder’ stage copy over the artifacts in dist folder to default nginx public folder
COPY --from=builder /app/out/wwwroot .
RUN ls -l /usr/share/nginx/html/
