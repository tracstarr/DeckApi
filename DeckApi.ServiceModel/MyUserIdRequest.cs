using ServiceStack;

namespace DeckApi.ServiceModel;

[Route("/my-user-id", "GET", Summary = "Get the current user's ID - convenience for testing since the user Id is not an int.")]
public class MyUserIdRequest: IGet, IReturn<MyUserIdResponse>
{
}

public class MyUserIdResponse
{
    public string UserId { get; set; }
}