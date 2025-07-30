# Home Assignment

This repository contains the solution for a home assignment focused on testing a .NET microservice.

## Tech Stack

The project is built and tested using the following technologies:

*   **.NET 8.0** & **ASP.NET Core**: For the core application logic and API.
*   **xUnit**: As the primary test runner for both unit and integration tests.
*   **Reqnroll**: For writing BDD-style, human-readable integration tests.
*   **Coverlet & ReportGenerator**: For collecting code coverage metrics and generating reports.
*   **Docker**: For creating a consistent, reproducible environment to run the entire test suite.

## Running the Tests

The entire test pipeline is managed via Docker Compose.

### Prerequisites
*   [.NET 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) installed.
*   [Docker Desktop](https://www.docker.com/products/docker-desktop/) installed and running.

### Instructions

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/Le0n-c0de/qa-home-assignment.git
    cd qa-home-assignment
    ```

2.  **Run the tests:**
    Execute the following command from the root of the project. This will build the Docker image, run all tests, and then exit, returning you to the terminal.

    ```bash
    dotnet build
    docker compose up --build
    ```

## Test Coverage

The project meets the **80% unit test coverage** requirement. The test script generates a detailed report after each run.

You can view the results in several ways:

1.  **In the Terminal**: The final line coverage percentage is printed directly to the console at the end of the test run.
2.  **Text Summary**: For a quick overview, open the `test-results/coverage-summary/Summary.txt` file.
3.  **Detailed HTML Report**: For a full report, open the `test-results/coverage-report/index.html` file in your web browser.

## Architectural Choices

### In-Memory Integration Testing

For the integration tests, `WebApplicationFactory` is used. This approach was chosen because it allows the tests to make requests directly to the application's request pipeline in-memory.

This has several advantages over testing against a running `localhost` instance:
*   **Speed**: No real network requests are made, which significantly speeds up test execution.
*   **Reliability**: Tests are isolated and not dependent on external factors like port availability.
*   **Simplicity**: It removes the need to manage the lifecycle of a separate web server process (Kestrel).

---

## Original Task Description (For Reference)

> # Home Assignment
>
> You will be required to write unit tests and automated tests for a payment application to demonstrate your skills.
>
> # Application information
>
> It’s an small microservice that validates provided Credit Card data and returns either an error or type of credit card application.
>
> # API Requirements
>
> API that validates credit card data.
>
> Input parameters: Card owner, Credit Card number, issue date and CVC.
>
> Logic should verify that all fields are provided, card owner does not have credit card information, credit card is not expired, number is valid for specified credit card type, CVC is valid for specified credit card type.
>
> API should return credit card type in case of success: Master Card, Visa or American Express.
>
> API should return all validation errors in case of failure.
>
>
> # Technical Requirements
>
>  - Write unit tests that covers 80% of application
>  - Write integration tests (preferably using Reqnroll framework)
>  - As a bonus:
>     - Create a pipeline where unit tests and integration tests are running with help of Docker.
>     - Produce tests execution results.
>
> # Running the  application
>
> 1. Fork the repository
> 2. Clone the repository on your local machine
> 3. Compile and Run application Visual Studio 2022.
