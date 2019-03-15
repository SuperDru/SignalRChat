FROM java

WORKDIR /app/liquibase

RUN apt-get update > /dev/null
RUN wget https://github.com/liquibase/liquibase/releases/download/liquibase-parent-3.6.3/liquibase-3.6.3-bin.tar.gz
RUN wget https://jdbc.postgresql.org/download/postgresql-42.2.5.jar
RUN tar xzvf liquibase-3.6.3-bin.tar.gz

COPY ./src/Database/Migrations ./Migrations
COPY ./run-migrations.sh ./
RUN chmod 777 ./run-migrations.sh
ENTRYPOINT ./run-migrations.sh