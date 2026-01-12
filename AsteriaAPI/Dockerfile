FROM eclipse-temurin:22-jdk-alpine

WORKDIR /target

COPY target/*.jar AsteriaAPI-0.0.1-SNAPSHOT.jar

EXPOSE 8080

ENTRYPOINT ["java", "-jar", "AsteriaAPI-0.0.1-SNAPSHOT.jar"]
