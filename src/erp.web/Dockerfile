FROM node:18-alpine AS build
WORKDIR /app

COPY src/erp.web/package*.json ./
RUN npm ci

COPY src/erp.web/ ./
RUN npm run build

FROM nginx:1.27-alpine AS runtime
COPY src/erp.web/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80
