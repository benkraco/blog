using backend.Repositories;

namespace backend.Services;

public class UserService
{
    private readonly UserRepository _userRepository;

    public UserService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _userRepository.GetByUsernameAsync(username);
    }

    public async Task<User?> LoginAsync(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);

        if (user is null) return null;

        var validPassword = BCrypt.Net.BCrypt.Verify(
            password,
            user.PasswordHash);

        if (!validPassword) return null;

        return user;
    }
}