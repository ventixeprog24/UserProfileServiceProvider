Partly AI Generated.

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

### Possible Return Codes
| Method                 | StatusCode | Message                              | Meaning                                                    |
| ---------------------- | :--------: | ------------------------------------ | ---------------------------------------------------------- |
| **CreateUserProfile**  |     201    | “User Profile Created”               | Successfully created a new profile                         |
|                        |     400    | “Bad Request”                        | Payload was invalid (factory returned `null`)              |
|                        |     500    | “Internal Server Error”              | Exception occurred while saving or caching                 |
| **GetUserProfileById** |     200    | –                                    | Profile found; returned in `Profile` field                 |
|                        |     400    | –                                    | `UserId` was null, empty, or whitespace                    |
|                        |     404    | –                                    | No profile exists with the given `UserId`                  |
|                        |     500    | –                                    | Exception during mapping from entity to model              |
| **GetAllUserProfiles** |      –     | –                                    | Always returns a `Profiles` message; may be empty on error |
| **UpdateUser**         |     200    | “User profile updated successfully.” | Successfully updated existing profile                      |
|                        |     404    | “Not found.”                         | No existing profile with the given `UserId`                |
|                        |     500    | “Internal server error”              | Exception occurred while loading, updating, or saving      |
| **DeleteUser**         |     200    | “User successfully deleted”          | Successfully removed the profile and its address           |
|                        |     400    | “Bad request”                        | `UserId` was null, empty, or whitespace                    |
|                        |     404    | “Not found”                          | No profile or address found for the given `UserId`         |
|                        |     500    | “Internal server error”              | Exception occurred during lookup or deletion               |

