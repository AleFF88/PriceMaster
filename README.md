## Technical Specification

### 1. General Information
* **Project Name:** PriceMaster .
* **Project Goal:** To create a reliable, scalable database for storing information about finished products, their components, composition, pricing, and production history.
* **Date of Compilation:** 11 dec 2025.
* **Nature of the Project:** This database design and implementation serves a demonstrative and didactic purpose. The goal is not commercial production, but rather the creation of a clear, well-structured, and robust system to be utilised as a key teaching aid in possible forthcoming technical publications (articles/book).

### 2. Functional Requirements
The database must ensure the following functionalities:
* **Finished Product Management:** Storing core characteristics (code, series, dimensions, recommended price).
* **Component Management (Materials/Labor):** Storing a list of all used components with their unit of measure and unit price.
* **BOM Generation:** The ability to precisely define the composition of each product (which component, in what quantity).
* **Historical Tracking:** Storing production history, including the final calculated price and the recommended price at the time of manufacturing.
* **Reporting:** Providing data to generate all planned reports (see Section 5).

### 3. Structure and Entity Descriptions (Tables)
> **Note:** Table and field names (SQL) will be based on the final, pluralized version.

#### 3.1. Products
* **ProductId** (PK)
* **ProductCode** (Unique identifier: 110, 510)
* **SeriesId** (FK > Series)
* **SizeWidth** (in centimeters) 
* **SizeHeight** (in centimeters)
* **RecommendedPrice** (UAH, may change over time) 
* **Notes** 

#### 3.2. Series (Series Reference)
* **SeriesId** (PK)
* **SeriesName** ("Cossacks. Birth Of Liberty.", "UNR 1917-1921. The Ukrainian Liberation Struggle.", "Kyivan Rus. The Golden Legacy.") 

#### 3.3. Units (Units of Measure Reference)
* **UnitId** (PK)
* **UnitName** (piece, m2)

#### 3.4. Categories (Component Type Reference)
* **CategoryId** (PK)
* **CategoryName** (e.g., Artifact, BaseMaterial, AssemblyWork) 

#### 3.5. Components (All Product Parts)
* **ComponentId** (PK)
* **ComponentName** (e.g., button, buckle, glue, labor)
* **UnitId** (FK > Units) 
* **PricePerUnit** (UAH) 
* **CategoryId** (FK > Categories) 

#### 3.6. BOMItems (Bill of Materials)
* **BOMItemId** (PK)
* **ProductId** (FK > Products) 
* **ComponentId** (FK > Components) 
* **Quantity** (e.g., 2 pieces, 0.5 units of glue) $

#### 3.7. ProductionHistory
* **ProductionHistoryId** (PK)
* **ProductId** (FK > Products) 
* **CreatedAt** (Date/time of production) 	
* **Price** (Final calculated product cost) 
* **RecommendedPrice** (Recommended price at the time of creation) 
* **WorkCost** (Cost of labor included in the price) 				
* **Notes** 

### 4. Relationship Requirements
All relationships must be established using Foreign Keys (FKs).

### 5. Reporting Requirements 
The database must support the efficient execution of the following queries/reports:
* List of products with their recommended price.
* A specific product with its detailed composition (BOM) and calculated cost.
* Displaying the materials reference, including unit price and unit of measure.
* Statistics:
    * Total items produced.
    * Items produced within a specific period.
    * Items produced by a specific type/category.
    * Items produced by a specific series.
    * Spent on WorkCost for a certain period

### 6. Technical Requirements
* **DBMS:** [SQLite].
* **DBMS:** **SQLite**. (The database solution for this project will utilize SQLite, primarily due to its lightweight and serverless nature, suitable for initial deployment.)
* **Backend Technology:** **ASP.NET Core** (initially using the **Minimal API** approach).
* **Future Planning:** The architecture must allow for a potential seamless migration to a more comprehensive ASP.NET Core framework, such as **ASP.NET Core MVC** (Model-View-Controller) or **Razor Pages**, should the need for increased complexity, structure, or extensive UI logic arise.
* **Encoding:** UTF-8.
* **Indexing:** Indexes must be created for all FK fields and fields frequently used for searching/filtering (`ProductCode`, `CreatedAt`).