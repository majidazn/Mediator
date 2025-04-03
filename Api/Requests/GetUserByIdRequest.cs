using Api.Dtos;
using Mediator;

namespace Api.Requests
{
    public class GetUserByIdRequest : IRequest<UserDto>
    {
        public int UserId { get; set; }
    }
    public class GetUserByIdHandler : IRequestHandler<GetUserByIdRequest, UserDto>
    {
        public Task<UserDto> Handle(GetUserByIdRequest request, CancellationToken cancellationToken)
        {
            // اطلاعات کاربر را برمی‌گرداند (اینجا فقط یک شبیه‌سازی است)
            var user = new UserDto { Id = request.UserId, Name = "Majid" };
            return Task.FromResult(user);
        }
    }

}
