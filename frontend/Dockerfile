FROM node:18.14.1-alpine AS build

WORKDIR /app/build
COPY package*.json ./
RUN npm ci

COPY . .
RUN npm run build

FROM node:18.14.1-alpine AS serve
WORKDIR app
COPY --from=build /app/build .
COPY --from=build /app/build/package.json .
COPY --from=build /app/build/node_modules ./node_modules
EXPOSE 3000
CMD ["node", "build/index.js"]