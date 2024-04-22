# RestFire

RestFire is a streamlined library designed to simplify interactions with Firebase's Realtime Database using a REST-like API. It offers an intuitive approach for developers who are familiar with RESTful principles, enabling them to seamlessly manage data within their Firebase projects

# Key Features
 * Authentication: Facilitates user sign-in, sign-up, and account deletion.
 * CRUD Operations: Provides CRUD (Create, Read, Update, Delete) functionality for managing data within Realtime Database collections.


# Installation
 `For now, just download the source code and integrate it into your project`


 # Usage

  1. Initialization:

  * Import the necessary namespaces in your C# code:
    ```csharp
    using RestFire;
    using RestFire.Http.Controllers;
    using RestFire.Types;
    ```

  * Create a RestFireApp instance, providing your Firebase project's Web API key and Realtime Database URL:
    ```csharp
    RestFireApp.Create(
     "YOUR_API_KEY",
     "YOUR_PROJECT_ID"
    );

    ```

<hr>

  2. Usage Without Authentication:
    * Instantiate a `RestFireBaseController` for a specific collection:
      ```csharp
      var sellersController = new RestFireBaseController<Seller>("Sellers");

      // Retrieve All
      var sellers = await sellersController.GetAllAsync();
      foreach (var seller in sellers)
      {
          Console.WriteLine(seller);
      }
      
      // Create
      var newSeller = new Seller { Id = Id.Next(), FullName = "...", ... };
      var insertResult = await sellersController.InsertAsync(newSeller);

      // Update
      var sellerToUpdate = new Seller { Id = "...", FullName = "...", ... };
      var updateResult = await sellersController.UpdateAsync(sellerToUpdate);

      // Delete by ID
      var deleteResultById = await sellersController.DeleteAsync("SELLER_ID");

      // Delete by Object
      var deleteResultByObject = await sellersController.DeleteAsync(sellerToDelete);
      ```
      
<hr>


  3. Usage With Authentication:
       * Perform authentication using `RestFireApp.Auth`:
       ```csharp
       var signInResult = await RestFireApp.Auth.SigninAsync("user@example.com", "password");

      if (signInResult.Success)
      {
          // Access authorized collections here
          var customersController = new RestFireBaseController<Customer>("Customers");
      
          var customer = await customersController.DeleteAsync("CUSTOMER_ID");
          Console.WriteLine(customer);
      }
       ```

## Additional Notes

  * Replace `YOUR_API_KEY` and `YOUR_PROJECT_ID` with your actual Firebase project credentials.
  * Ensure you have a valid internet connection to communicate with the Firebase Realtime Database.
  * Refer to the library's documentation for more detailed information on available methods, properties, and advanced usage scenarios (if available).

By following these guidelines, you can effectively leverage the RestFire library to manage your Firebase Realtime Database data in a RESTful manner, streamlining your development process.
































    
