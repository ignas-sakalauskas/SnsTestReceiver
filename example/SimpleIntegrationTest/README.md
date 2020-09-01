# Simple Integration Test
The test sends a simple object to SNS, and asserts the received message contains exactly the same object.

## How to run?

1. Go to the `SimpleIntegrationTest` folder in the examples in command prompt
2. Load dependencies in docker with `docker-compose up`
3. Run tests with `dotnet test`