# Project 4 : Summary

In this project, we will develop a REST API application for managing inventory and products.  
  
The application will interact with a SQL Server database. All SQL files can be found in Data folder.   
  
Here are the main tasks involved:

1. Create a new REST API application.
2. Add a controller named `WarehousesController`.
3. Implement an endpoint to handle the following request:  
   a) The endpoint should respond to an HTTP POST request to `/api/warehouses`.  
   b) It should receive data in the following format:  
   ```
   {
     "ProductId": int,
     "WholesalerId": int,
     "Amount": int
   }
   ```
   c) All fields are required, and the `Amount` must be greater than 0.  
   d) The endpoint should follow the action scenario described below.  

Action Scenario: Product Registration with the Wholesaler
Main Scenario:
1. Check if the product with the given `ProductId` exists and if the wholesaler with the specified `WholesalerId` exists. Also, ensure that the `Amount` value is greater than 0.
2. Add the product to the wholesaler only if there is a corresponding order in the `Order` table to purchase the product. Check if there is a record in the `Order` table with `IdProduct` and `Amount` matching the request. The `CreatedAt` of the order should be earlier than the `CreatedAt` from our request.
3. Check if the order has already been processed by checking if there is a row in the `Product_Warehouse` table with the given `IdOrder`.
4. Update the `FulfilledAt` column of the order row to mark the order as fulfilled with the current date and time (UPDATE).
5. Insert a record into the `Product_Warehouse` table. The `Price` column should contain the multiplied price of a single product with the `Amount` value from the request. Additionally, insert a `CreatedAt` value consistent with the current time (INSERT).
6. Return the value of the primary key generated for the inserted record in the `Product_Warehouse` table as the result of the operation.

Alternative Scenario:  
1a. If the product or wholesaler with the given id does not exist, return a 404 error with the corresponding message.  
2a. If there is no corresponding order, return a 404 error with the corresponding message.  
3a. If the order has already been processed, return the corresponding error code with a message.  

Additionally, add a second `Warehouses2Controller` with a stub to handle requests sent to the HTTP POST address `/api/warehouses2`. The endpoint should implement the same logic but utilize a stored procedure (provided in the `proc.sql` file).

Remember to handle proper dependency injection, use appropriate names, return correct HTTP status codes, and consider using asynchronous programming with `async/await` where applicable.
