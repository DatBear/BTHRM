FROM node:21.1.0 as build

WORKDIR /next-app

ARG ENV_FILE

COPY bthrm.web/package*.json .
COPY bthrm.web/yarn.lock .
RUN yarn --frozen-lockfile
COPY ./bthrm.web/. .
COPY bthrm.web/${ENV_FILE} ./.env
RUN yarn build

FROM node:21-alpine as run
WORKDIR /next-app
COPY --from=build /next-app/package.json .
COPY --from=build /next-app/yarn.lock .
COPY --from=build /next-app/next.config.mjs ./
COPY --from=build /next-app/public ./public
COPY --from=build /next-app/.next/standalone ./
COPY --from=build /next-app/.next/static ./.next/static
ENV PORT 8080
EXPOSE 8080
ENTRYPOINT ["node", "server.js"]