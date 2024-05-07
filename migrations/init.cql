-- Create the keyspace with SimpleStrategy. Application is deployed in a single data center or a single cloud region.

CREATE KEYSPACE IF NOT EXISTS devBlockchainData
WITH replication = {
    'class': 'SimpleStrategy',
    'replication_factor': 1
};

-- Use the keyspace
USE devBlockchainData;

-- Create the table
CREATE TABLE IF NOT EXISTS blockchain (
    name text,
    createdAt timestamp,
    height int,
    hash text,
    time timestamp,
    latestUrl text,
    previousHash text,
    previousUrl text,
    peerCount int,
    unconfirmedCount int,
    highFeePerKb int,
    mediumFeePerKb int,
    lowFeePerKb int,
    highGasPrice bigint,
    mediumGasPrice bigint,
    lowGasPrice bigint,
    lastForkHeight int,
    lastForkHash text,
    PRIMARY KEY (name, createdAt)
) WITH CLUSTERING ORDER BY (createdAt DESC);

