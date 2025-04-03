# **Custom Mediator**  
A lightweight **CQRS Mediator** implementation in C# without dependencies like `MediatR`. This library helps in decoupling request handling logic and supports **automatic dependency injection** of handlers.  

---

## **Features**  
✅ Supports **Request/Response** pattern (`IRequest<TResponse>`)  
✅ Supports **Publish/Subscribe** pattern (`INotification`)  
✅ Uses **Reflection-Free Invocation** for performance optimization  
✅ **Automatic Handler Registration** with `Scrutor`  
✅ Works with **ASP.NET Core Dependency Injection**  

---

## **Installation**  

1️⃣ **Add the project to your solution** or create a separate class library.  
2️⃣ **Install dependencies** (if using automatic DI registration):  
```sh
dotnet add package Scrutor
```
3️⃣ **Register the mediator in your DI container** (for ASP.NET Core):  
```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCustomMediator();
var app = builder.Build();
app.Run();
```

---

## **Usage**  

### **1️⃣ Define a Request and a Handler**  
#### Request (Query or Command)
```csharp
public class GetUserByIdRequest : IRequest<UserDto>
{
    public int Id { get; set; }
    public GetUserByIdRequest(int id) => Id = id;
}
```
#### Handler  
```csharp
public class GetUserByIdHandler : IRequestHandler<GetUserByIdRequest, UserDto>
{
    public Task<UserDto> Handle(GetUserByIdRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new UserDto { Id = request.Id, Name = "John Doe" });
    }
}
```

### **2️⃣ Use the Mediator in a Controller or Service**  
```csharp
[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUserById(int id)
    {
        var user = await _mediator.Send(new GetUserByIdRequest(id));
        return user == null ? NotFound() : Ok(user);
    }
}
```

### **3️⃣ Define and Handle Notifications (Events)**  
#### Notification  
```csharp
public class UserCreatedEvent : INotification
{
    public string Username { get; }
    public UserCreatedEvent(string username) => Username = username;
}
```
#### Notification Handler  
```csharp
public class UserCreatedHandler : INotificationHandler<UserCreatedEvent>
{
    public Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"User {notification.Username} was created.");
        return Task.CompletedTask;
    }
}
```
#### Publishing the Event  
```csharp
await _mediator.Publish(new UserCreatedEvent("JohnDoe"));
```

---

## **Performance Optimizations**  
🚀 **No Reflection.Invoke()** – Uses `Delegate.CreateDelegate()` for faster execution.  
🚀 **Auto-Registration of Handlers** – No need to manually register handlers in `Program.cs`.  
🚀 **Thread-Safe & Efficient** – Uses `Task.WhenAll()` for parallel execution of notifications.  

---

## **Contributing**  
Feel free to submit pull requests or suggestions to improve this lightweight mediator.  

---

## **License**  
This project is open-source and available under the MIT License.  

---

💡 **Inspired by MediatR but designed for lightweight projects that need better performance!** 🚀
