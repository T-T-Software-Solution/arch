using Shopping.Shared.Entities.ViewModels;

namespace Shopping.WebApi.Biz.Users.ViewModels;

public sealed record CreateUserResult(UserVm UserInfo, string Token);