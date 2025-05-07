# UserProfileServiceProvider

## How to use it

Make sure you have an exact copy of the proto-file in the client application. Don't forget to register the proto in the project.

### Install following packages in the client:
- Google.Protobuf
- Grpc.Net.Client
- Grpc.Net.ClientFactory
- Grpc.Tools

### Example use

```
using var channel = GrpcChannel.ForAddress("https://localhost:5001");
var client = new UserProfileService.UserProfileServiceClient(channel);

//Create UserProfile
var createReply = await client.CreateUserProfileAsync(new UserProfile {
  UserId = Guid.NewGuid().ToString(),
  FirstName = "Alice",
  LastName = "Smith",
  Email = "alice.smith@example.com",
  PhoneNumber = "+1234567890",
  Address = new StringValue { Value = "123 Main St" },
  PostalCode = new StringValue { Value = "12345" },
  City = new StringValue { Value = "Metropolis" }
  });

//Get UserProfile by Id
var getReply = await client.GetUserProfileByIdAsync(new RequestByUserId {
  UserId = "the-same-guid-used-above"
  });

//Get all userProfiles
var allReply = await client.GetAllUserProfilesAsync(new Empty());


// Update an existing user
var updateReply = await client.UpdateUserAsync(new UserProfile {
  UserId = "the-same-guid-used-above",
  FirstName = "Alice",
  LastName = "Johnson",      // changed last name
  Email = "alice.j@example.com",
  PhoneNumber = "+1234567890",
  Address = new StringValue { Value = "456 Elm St" },
  PostalCode = new StringValue { Value = "67890" },
  City = new StringValue { Value = "Gotham" }
  });

// Delete a user profile
var deleteReply = await client.DeleteUserAsync(new RequestByUserId {
  UserId = "the-same-guid-used-above"
  });
```
