# Smart-Document-Management

![GitHub license](https://img.shields.io/github/license/YOUR_USERNAME/YOUR_REPOSITORY_NAME?style=flat-square)
![.NET Core](https://img.shields.io/badge/.NET_Core-512BD4?style=for-the-badge&logo=.net&logoColor=white)
![MySQL](https://img.shields.io/badge/MySQL-4479A1?style=for-the-badge&logo=mysql&logoColor=white)
![HTML5](https://img.shields.io/badge/HTML5-E34F26?style=for-the-badge&logo=html5&logoColor=white)
![CSS3](https://img.shields.io/badge/CSS3-1572B6?style=for-the-badge&logo=css3&logoColor=white)
![JavaScript](https://img.shields.io/badge/JavaScript-F7DF1E?style=for-the-badge&logo=javascript&logoColor=black)

## Table of Contents

* [About](#about)
* [Features](#features)
* [Technologies Used](#technologies-used)
* [Getting Started](#getting-started)
    * [Prerequisites](#prerequisites)
    * [Installation](#installation)
    * [Database Setup](#database-setup)
* [Usage](#usage)
* [API Key Configuration](#api-key-configuration)

---

## About

The **Document Insight Hub** is a powerful web-based application designed to streamline document management and information extraction. It allows users to upload PDF and plain text documents, leveraging the power of AI to automatically process, categorize, and summarize their content. With its intelligent semantic search capabilities, finding relevant information across a vast collection of documents has never been easier. The system also provides comprehensive CRUD (Create, Read, Update, Delete) functionalities for efficient document management.

---

## Features

* **File Upload:** Securely upload PDF and plain text (`.txt`) documents.
* **AI Text Processing:**
    * Automatically extracts key information from uploaded documents.
    * Utilizes AI for intelligent categorization of document content.
* **Summary Generation:** Generates concise, AI-powered summaries for each uploaded document, providing quick insights.
* **Semantic Search:** Perform intelligent, context-aware searches across all stored documents, going beyond keyword matching to understand user intent.
* **Data Storage:**
    * Stores both essential document metadata (e.g., title, upload date, category) and the full document text in a MySQL database.
* **CRUD Operations:** Full control over your documents with basic Create, Read, Update, and Delete functionalities.

---

## Technologies Used

This project is built using a robust stack of modern technologies:

* **Backend:**
    * **.NET Core:** A cross-platform, high-performance framework for building scalable web applications.
    * **Gemma API (via Google Cloud or similar service):** Utilized for AI-driven text processing, summarization, and categorization. (Note: Specific integration details for Gemma would depend on the chosen Google Cloud service or direct API access).
    * **UglyToad.PdfPig:** A .NET library used for parsing PDF documents and extracting text content.
* **Database:**
    * **MySQL:** A popular open-source relational database management system for storing document data and metadata.
* **Frontend:**
    * **HTML:** For structuring the web pages.
    * **CSS:** For styling the user interface.
    * **JavaScript:** For interactive elements and client-side logic.

---

## Getting Started

Follow these instructions to set up and run the Document Insight Hub on your local machine.

### Prerequisites

Before you begin, ensure you have the following installed:

* **.NET SDK (8.0 or newer recommended):** Download from [dotnet.microsoft.com](https://dotnet.microsoft.com/download)
* **MySQL Server:** Download from [dev.mysql.com/downloads/mysql/](https://dev.mysql.com/downloads/mysql/)
* **MySQL Workbench (Optional but recommended):** For easy database management.
* **A text editor or IDE (e.g., Visual Studio Code, Visual Studio):** For code editing.
* **Gemma API Key:** Obtain an API key for the Gemma model. This typically involves setting up a project on Google Cloud Platform and enabling the necessary APIs. Refer to the official Google Cloud documentation for obtaining a Gemma API key.

### Installation

1.  **Clone the repository:**

    ```bash
    git clone https://github.com/SASIN83/Smart-Document-Management.git
    cd Smart Document Management
    ```

2.  **Restore .NET dependencies:**

    ```bash
    dotnet restore
    ```

### Database Setup

1.  **Create a MySQL database:**

a. Open MySQL Workbench or your preferred MySQL client

b. Import this DB https://github.com/SASIN83/Smart-Document-Management/blob/main/Database%20and%20HTML/DB%20Structure.sql
    

details:


---

## Usage

1.  **Run the application:**

    ```bash
    dotnet run
    ```
    This will start the .NET Core web application, typically on `http://localhost:5000` or `http://localhost:5001` (HTTPS).

2.  **Access the application:**
    Open your web browser and navigate to the URL displayed in your terminal (e.g., `http://localhost:5000`).

3.  **Upload Documents:**
    Use the "Upload" section of the application to select and upload your PDF or `.txt` files. The system will automatically process them.

4.  **View and Manage Documents:**
    Browse your uploaded documents, view their summaries, categories, and perform CRUD operations.

5.  **Perform Semantic Search:**
    Use the search bar to intelligently query your documents. Try natural language questions or phrases for more accurate results.

---

## API Key Configuration

The application relies on the Gemma API for AI functionalities. You **must** configure your Gemma API key before running the application.
1. Open config.xml
2. Copy your Gemini API Key in API-KEY of OTHER section

The application will attempt to load the API key from these locations.

---
