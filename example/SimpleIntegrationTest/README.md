# Simple Integration Test
The test sends a simple object to SNS, and asserts the received message contains exactly the same object.

## How to run?

1. Go to the `SimpleIntegrationTest` folder in the examples in command prompt
2. Load dependencies in docker with `docker-compose up` (might need to grant execute permissions for `init.sh` with `chmod +x init.sh` if you are on Mac/Linux)
3. Run tests with `dotnet test`