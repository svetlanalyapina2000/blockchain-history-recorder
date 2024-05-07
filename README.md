# Blockchain History Recorder

The Blockchain History Recorder is a solution designed to retrieve and store data for various blockchains.

## Features

- **Fetch Historical Blocks**: Retrieve detailed information about past blocks in a blockchain.
- **Store Blockchain Data**: Persist fetched blockchain data in a structured format for future queries.
- **Query Blockchain History**: Provides endpoints to query stored historical data efficiently.

## Prerequisites
- .NET 8.0 SDK or later
- Docker (container deployment)

## Getting Started

### Installation

1. **Clone the repository:**

   ```bash
   git clone https://github.com/svetlanalyapina2000/blockchain-history-recorder.git
   cd blockchain-history-recorder
   ```

2. **Using Docker:**

   ```bash
   docker-compose up -d 
   ```
- Before interacting with the Blockchain History Service, ensure that the `blockchain_app` Docker container is fully up and running.
- Avoid restarting `cassandra` containers as their initial startup may require some time.

3. **Local Configuration:**

- To run the Blockchain History Service locally, you need to configure certain settings in the `appsettings.json` file. These settings ensure that the application can connect to necessary services like Cassandra and properly manage CORS (Cross-Origin Resource Sharing) for your development environment.

### Update `appsettings.json`

Add the following configuration to your `appsettings.json` file under the root object:

```json
{
  "ALLOWED_ORIGINS": "http://localhost*",
  "CASSANDRA_HOST": "localhost",
  "CASSANDRA_PORT": 9042,
  "CASSANDRA_KEYSPACE": "devblockchaindata",
  "APP_HOST": "0.0.0.0",
  "APP_PORT": 30123
}
```

### Accessing the API
- Once the service is running, you can access the Swagger UI by navigating to the following URL:
  [Swagger UI Documentation](http://localhost:30122/swagger) (http://0.0.0.0:30122/api/docs/index.html)
- The application will start and be accessible depending on the APP_PORT defined in your appsettings.json or docker-compose.yml.


### Temporary Testing Containers

It's important to note that [Testcontainers](https://www.testcontainers.org/) may create additional Docker containers during test execution. 

### Manual Cleanup

Occasionally, some containers may not be properly removed after testing due to unforeseen errors or interruptions during the test process. If you find that there are leftover containers, you can manually remove them to free up resources and maintain a clean environment.

