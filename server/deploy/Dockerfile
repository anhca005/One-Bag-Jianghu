FROM maven:3.9.9-eclipse-temurin-17 AS build
WORKDIR /build
COPY server/pom.xml server/mvnw server/mvnw.cmd ./
COPY server/.mvn .mvn
COPY server/battle-engine/pom.xml battle-engine/pom.xml
COPY server/game-server/pom.xml game-server/pom.xml
COPY server/battle-engine/src battle-engine/src
COPY server/game-server/src game-server/src
COPY server/battle-engine/samples battle-engine/samples
RUN ./mvnw -B -pl game-server -am package -DskipTests

FROM eclipse-temurin:17-jre-jammy
RUN apt-get update \
    && apt-get install --yes --no-install-recommends sqlite3 \
    && rm -rf /var/lib/apt/lists/* \
    && groupadd --system jianghu \
    && useradd --system --gid jianghu --home-dir /app jianghu
WORKDIR /app
COPY --from=build /build/game-server/target/game-server.jar /app/game-server.jar
COPY --from=build /build/battle-engine/samples /app/samples
COPY One-Bag-Jianghu/Assets/_Game/Resources/Data /app/data
RUN mkdir --parents /app/state/backups \
    && chown --recursive jianghu:jianghu /app
USER jianghu
EXPOSE 8765
CMD ["java", "-jar", "/app/game-server.jar", "--data", "/app/data"]
