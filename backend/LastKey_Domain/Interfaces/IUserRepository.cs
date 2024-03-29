﻿using LastKey_Domain.Entities;

namespace LastKey_Domain.Interfaces;

public interface IUserRepository
{
    Task<User> CreateUserAsync(User user);

    Task<User?> GetUserByUsernameAsync(string username);
    
    Task<User?> GetUserInfoByIdAsync(int userId);
    
    Task<bool> UsernameExistsAsync(string username);

    List<User> RetrieveMembersByUserId(int userId);

    Task<bool> DeleteUserAsync(int userId, int adminId);
}