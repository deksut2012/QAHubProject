FROM node:24-alpine AS dependencies
WORKDIR /app
COPY apps/web/package*.json ./
RUN npm ci

FROM node:24-alpine AS build
WORKDIR /app
COPY --from=dependencies /app/node_modules ./node_modules
COPY apps/web/ .
RUN npm run build

FROM node:24-alpine AS final
WORKDIR /app
ENV NODE_ENV=production
COPY --from=build /app/package.json /app/package-lock.json ./
COPY --from=build /app/.next ./.next
COPY --from=build /app/public ./public
COPY --from=build /app/node_modules ./node_modules
EXPOSE 3000
USER node
CMD ["npm", "run", "start"]
