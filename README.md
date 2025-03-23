# Staticsoft Contracts

A type-safe HTTP contract library for ASP.NET Core applications that simplifies API development by providing a clean, declarative approach to defining and consuming HTTP endpoints.

## Overview

Staticsoft Contracts is a library that enables a contract-first approach to building HTTP APIs in .NET. It provides a way to define API contracts as C# interfaces and classes, which can then be used to generate both server-side endpoint handlers and client-side API consumers.

The library promotes a clean separation between API contracts and their implementations, making it easier to maintain and evolve APIs over time.

## Key Features

- **Type-safe API contracts**: Define your API using C# classes and interfaces with strong typing
- **Automatic routing**: Routes are automatically generated based on the structure of your API contract
- **Client-side API generation**: Automatically generate client-side API consumers from your contracts
- **Support for parametrized endpoints**: Easily define endpoints with URL parameters
- **Custom HTTP status codes**: Configure custom status codes for specific endpoints
- **Dependency injection integration**: Seamless integration with ASP.NET Core's dependency injection system

## Getting Started

### Defining an API Contract

```csharp
// Define your API entry point
public class MyAPI
{
    public MyAPI(UserGroup userGroup)
    {
        UserGroup = userGroup;
    }

    public UserGroup UserGroup { get; }
}

// Define an API group
public class UserGroup
{
    public UserGroup(
        HttpEndpoint<CreateUserRequest, UserResponse> createUser,
        ParametrizedHttpEndpoint<EmptyRequest, UserResponse> getUser
    )
    {
        CreateUser = createUser;
        GetUsers = getUsers;
        GetUser = getUser;
    }

    [Endpoint(HttpMethod.Post)]
    public HttpEndpoint<CreateUserRequest, UserResponse> CreateUser { get; }

    [Endpoint(HttpMethod.Get)]
    public ParametrizedHttpEndpoint<EmptyRequest, UserResponse> GetUser { get; }
}

// Define request and response models
public class CreateUserRequest
{
    public string Name { get; set; }
    public string Email { get; set; }
}

public class UserResponse
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}
```

### Implementing Endpoint Handlers (Server-side)

```csharp
// Implement the CreateUser endpoint
public class CreateUserHandler : HttpEndpoint<CreateUserRequest, UserResponse>
{
    private readonly UserRepository _repository;

    public CreateUserHandler(UserRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserResponse> Execute(CreateUserRequest request)
    {
        var user = await _repository.CreateUser(request.Name, request.Email);
        return new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email
        };
    }
}

// Implement the GetUser endpoint
public class GetUserHandler : ParametrizedHttpEndpoint<EmptyRequest, UserResponse>
{
    private readonly UserRepository _repository;

    public GetUserHandler(UserRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserResponse> Execute(string userId, EmptyRequest request)
    {
        var user = await _repository.GetUser(userId);
        return new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email
        };
    }
}
```

### Configuring the Server

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .UseServerAPI<MyAPI>(Assembly.GetExecutingAssembly())
            .AddHttpContextAccessor()
            .UseSystemJsonSerializer();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app
            .UseRouting()
            .UseServerAPIRouting<MyAPI>();
    }
}
```

### Consuming the API (Client-side)

```csharp
// Configure the client
var services = new ServiceCollection()
    .UseClientAPI<MyAPI>()
    .UseSystemJsonSerializer()
    .UseJsonHttpCommunication()
    .BuildServiceProvider();

// Get the API client
var api = services.GetRequiredService<MyAPI>();

// Call an endpoint
var user = await api.UserGroup.GetUser.Execute("123");
Console.WriteLine($"User: {user.Name}, Email: {user.Email}");
```

## Advanced Features

### Custom URL Patterns

You can customize the URL pattern for an endpoint:

```csharp
[Endpoint(HttpMethod.Get, pattern: "all")]
public HttpEndpoint<EmptyRequest, UsersResponse> GetAllUsers { get; }
```

### Custom Status Codes

You can specify a custom status code for an endpoint:

```csharp
[Endpoint(HttpMethod.Post)]
[EndpointBehavior(statusCode: 201)]
public HttpEndpoint<CreateUserRequest, UserResponse> CreateUser { get; }
```

### Nested Groups

You can nest API groups to create a hierarchical structure:

```csharp
public class UserGroup
{
    public UserGroup(
        HttpEndpoint<EmptyRequest, UsersResponse> getUsers,
        ProfileGroup profileGroup
    )
    {
        GetUsers = getUsers;
        ProfileGroup = profileGroup;
    }

    [Endpoint(HttpMethod.Get)]
    public HttpEndpoint<EmptyRequest, UsersResponse> GetUsers { get; }

    public ProfileGroup ProfileGroup { get; }
}

public class ProfileGroup
{
    public ProfileGroup(
        HttpEndpoint<UpdateProfileRequest, ProfileResponse> updateProfile
    )
    {
        UpdateProfile = updateProfile;
    }

    [Endpoint(HttpMethod.Put)]
    public HttpEndpoint<UpdateProfileRequest, ProfileResponse> UpdateProfile { get; }
}
```

## License

This project is licensed under the terms of the license included in the repository.
