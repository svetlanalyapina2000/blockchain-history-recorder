#!/bin/bash
while ! cqlsh $CASSANDRA_HOST $CASSANDRA_PORT -e 'describe cluster'; do
  echo 'Waiting for Cassandra to start...';
  sleep 2;
done;

# List and sort migration files
MIGRATION_FILES=$(ls $MIGRATION_DIR | grep '\.cql$' | sort)

# Execute each migration file
for file in $MIGRATION_FILES
do
  echo "Applying migration $file..."
  cqlsh $CASSANDRA_HOST $CASSANDRA_PORT  -f $MIGRATION_DIR/$file
done

echo 'Cassandra is up and running!'