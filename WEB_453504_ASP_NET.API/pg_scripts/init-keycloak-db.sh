#!/bin/bash
set -e

echo "Creating Keycloak database and granting privileges to user $POSTGRES_USER..."

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
    CREATE DATABASE keycloak OWNER $POSTGRES_USER;
    GRANT ALL PRIVILEGES ON DATABASE keycloak TO $POSTGRES_USER;
EOSQL