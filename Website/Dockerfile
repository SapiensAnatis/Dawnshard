FROM node:22.3.0-bookworm-slim AS base

EXPOSE 3000

ENV NPM_CONFIG_PREFIX=/home/node/.npm-global
ENV PATH=$PATH:/home/node/.npm-global/bin
RUN npm install -g pnpm

ENV PNPM_HOME="/pnpm"
ENV PATH="$PNPM_HOME:$PATH"

WORKDIR /app
COPY ["Website/package.json", "Website/pnpm-lock.yaml", "./"]

FROM base AS prod-deps
RUN --mount=type=cache,id=pnpm,target=/pnpm/store pnpm install --prod --frozen-lockfile

FROM base AS build
RUN --mount=type=cache,id=pnpm,target=/pnpm/store pnpm install --frozen-lockfile
COPY ["Website/", "./"]
# TODO: Consider accessing this parameter via import.meta.env instead.
RUN echo "PUBLIC_ENABLE_MSW=false" > ./.env.production 
RUN pnpm run build

FROM base
COPY --from=prod-deps ["/app/node_modules", "./node_modules"]
COPY --from=build ["/app/build", "./"]

ENV NODE_ENV="production"

USER node
CMD ["node", "./index.js"]