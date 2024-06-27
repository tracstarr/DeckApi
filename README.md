### Build and Run

```powershell
dotnet build
```
To run migrations and to do initial data seeding, run the following command:
```powershell
dotnet run --project DeckApi/DeckApi.csproj --AppTasks=migrate
```

To run the application, run the following command:
```powershell
dotnet run --project DeckApi/DeckApi.csproj
```

You can then get to the swagger endpoint at `https://localhost:5001/swagger/index.html`

### Run Tests

```powershell
dotnet test
```

### Some Notes

I decided to use tools that were familiar to me, which included the use of ServiceStack to help scaffold the API. 
Under the hood it still uses ASP.NET Core. 

I took the liberty of making some assumptions about the data structure to allow for more flexibility and
more real world scenarios. For example I added a Product table for the root items on the cart. This allows
for history tracking and snapshots the items in the card if a product was to change. 

I used Identity Auth since it's familiar to me - but this requires userIds to be strings and not ints - so
there was a change in the data structure to accommodate this.

Additionally, in the API endpoints I did keep with the requested request structure, but I would possibly
consider removing the UserId parameter since the endpoint is already authenticated and we have the 
userId from the session. 

Swagger is added and provides basic Auth to run the endpoints from the UI. 

``` 
User: shopper@emaiilcom
Password: p@55wOrd
```

There is also an added service to fetch your current logged in userId to make it easier for setting the userId on 
required endpoints. 

I've also added a http file with some basic requests to test the API. If you use rider you can run these. 

--Keith
