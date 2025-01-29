# Book Catalog Management System

This project is a full-stack web application developed as a test task for a Senior Full-Stack .NET Developer position. It allows users to manage and track a collection of books using a web interface.

## Overview

This application demonstrates a layered architecture with a clear separation of concerns. It features:

*   **A robust backend API** built with ASP.NET Core Minimal API for handling data persistence, business logic, and API endpoints.
*   **A dynamic frontend interface** built using Blazor Server, offering a responsive design for an improved user experience.

## Technologies Used

*   **Backend:**
    *   C#
    *   ASP.NET Core Minimal API (.NET 9.0)
    *   Entity Framework Core 9.0 (In-memory Database)
    *   Microsoft.AspNetCore.SignalR
    *   Microsoft.AspNetCore.RateLimiting
    *   Swashbuckle.AspNetCore
    *   CsvHelper
*   **Frontend:**
    *   Blazor Server (.NET 9.0)
    *   Bootstrap for styling
    *   Microsoft.AspNetCore.SignalR.Client
    *   Microsoft.JSInterop
    *   Microsoft.AspNetCore.Components.Forms
    *  Microsoft.AspNetCore.Components.Web
*   **Containerization:** Docker (with provided Dockerfile and docker-compose)
* **Version Control:** Git and GitHub

## Features

**Backend (`BCMS_API`)**

*   Implements standard CRUD operations for a `Book` entity (Create, Read, Update, Delete).
*   Provides API endpoints for managing books.
*   Supports filtering books by title, author, and genre using query parameters.
*   Implements pagination and sorting options for book listings.
*   Includes a CSV bulk upload endpoint for importing multiple books from a CSV file.
*   Implements rate limiting using a fixed window policy to safeguard the API from abuse.
*   Uses in-memory database for data storage, using Entity Framework Core.
*   Provides API documentation with Swagger.
* Uses `BookDto` as data transfer objects for api communication.
*   Implements antiforgery tokens to prevent Cross Site Request Forgery (CSRF) attacks.

**Frontend (`BCMS_UI`)**

*   Displays a list of books in a responsive table format using `BookTable` component.
*   Provides a single search input field with a dropdown to select the search parameter (title, author, or genre).
*   Enables users to add new books using `AddBookForm` component.
*   Enables users to update book details using an `EditBookModal` component.
*   Provides functionality to delete books.
*  Has a responsive design using bootstrap classes.
*   Implements real-time updates to the book list, using SignalR when changes are made to the data.
*   Handles file uploads via a dedicated file input, javascript interop, and a backend endpoint for CSV import.
*   Implements pagination for large data sets.
*  Implements sorting using a dropdown.
*  Implements a separate `Search` and `SortDropdown` component for better organization.
* Has a navigation menu, with proper navigation.

## Architecture Overview

The application is designed using a multi-layered approach to ensure proper separation of concerns:

*   **Presentation Layer (`BCMS_UI`):** This layer is responsible for handling the user interface and interaction. It uses Blazor Server to render the UI components on the server-side, and is also using JavaScript interop for client side access, also it handles all state management and all business requirements.
*   **Application Layer (`BCMS_UI/Services`):** This layer contains the core application logic, orchestrates data flow, and provides services used by UI components, and communicates with the backend api. It defines `BookService` which implements `IBookService` for accessing data. It also handles communication with the API, and performs some logic on the data.
*   **API Layer (`BCMS_API`):** This layer is responsible for handling all the backend logic and exposes all the functionalities using HTTP endpoints. It contains the implementation of CRUD, bulk upload, and other data processing methods.
*   **Data Access Layer (`BCMS_API/Data`):** This layer is responsible for the data persistence, and using `EF Core` to communicate with the database.
*   **Models:** Data models are present in the `Models` folder in both `BCMS_API` and `BCMS_UI` projects.

## Getting Started

To run this project locally, you will need to have the following installed:

*   [.NET SDK 9.0](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) or higher
*   [Docker Desktop](https://www.docker.com/products/docker-desktop) or Docker Engine

**Steps to Run the Application (Docker):**

1.  **Clone the Repository:**

    ```bash
    git clone https://github.com/romail/intsurfing_task
    ```

2.  **Navigate to the project root directory (where `docker-compose.yml` is located):**

    ```bash
    cd intsurfing_task
    ```

3.  **Build and Run the Application:**

    ```bash
    docker-compose up --build
    ```
This command will build the docker images for both your API and UI, and run all the containers.

4.  **Access the Application:**
    *   The Blazor UI will be available at `http://localhost:5254` in your browser.
    *   The API documentation (Swagger) will be available at `http://localhost:5202/swagger`.

**Steps to Run the Application (Without Docker):**

1.  **Navigate to the `BCMS_API` folder:**

    ```bash
     cd src/BCMS_API
    ```
2.  **Run the API application:**

    ```bash
    dotnet run
    ```
3.  **Open another terminal and navigate to the `BCMS_UI` folder:**

    ```bash
    cd src/BCMS_UI
    ```
4.  **Run the UI application:**

    ```bash
    dotnet run
    ```
5. **Access the Application:**

    *   The Blazor UI will be available at `http://localhost:5254` in your browser.
    * The API documentation (Swagger) will be available at `http://localhost:5202/swagger`

6.  **Import Test Data (Optional):** You can use the following CSV data for testing purposes:

    ```csv
    Title,Author,Genre
    The Lord of the Rings,J.R.R. Tolkien,Fantasy
    Pride and Prejudice,Jane Austen,Romance
    1984,George Orwell,Dystopian
    To Kill a Mockingbird,Harper Lee,Classic
    The Hitchhiker's Guide to the Galaxy,Douglas Adams,Science Fiction
    Dune,Frank Herbert,Science Fiction
    The Great Gatsby,F. Scott Fitzgerald,Classic
    The Catcher in the Rye,J.D. Salinger,Literary
    The Hobbit,J.R.R. Tolkien,Fantasy
    Jane Eyre,Charlotte Brontë,Gothic
    Little Women,Louisa May Alcott,Classic
    Animal Farm,George Orwell,Dystopian
    Brave New World,Aldous Huxley,Dystopian
    Foundation,Isaac Asimov,Science Fiction
    The Picture of Dorian Gray,Oscar Wilde,Gothic
    ```

## Code Structure

The projects are organized as follows:

*   **`src/BCMS_API`:** Contains the backend code (ASP.NET Core Minimal API project).
*   **`src/BCMS_UI`:** Contains the frontend code (Blazor Server application).
*   **`docker-compose.yml`**: This file is located at the root of your project, and is responsible for starting the services.
*   **`README.md`:** This is the documentation file, located at the root of the project.
