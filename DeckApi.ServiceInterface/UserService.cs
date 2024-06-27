using DeckApi.ServiceModel;
using ServiceStack;

namespace DeckApi.ServiceInterface;

public class UserService: Service
{
    public MyUserIdResponse Get(MyUserIdRequest request)
    {
        return new MyUserIdResponse { UserId = base.GetSession().UserAuthId };
    }
}  