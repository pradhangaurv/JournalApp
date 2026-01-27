using System;
using System.Threading.Tasks;
using JouralAppWeb.Database;

namespace JouralAppWeb.Services
{
    public class AuthService
    {
        private readonly AppDbContext _db;

        public User? CurrentUser { get; private set; }

        public AuthService(AppDbContext db)
        {
            _db = db;
        }

        public bool IsLoggedIn => CurrentUser != null;

        public int GetCurrentUserId()
        {
            if (CurrentUser == null)
                throw new InvalidOperationException("No user is logged in.");

            return CurrentUser.Id;
        }

        public void RequireLogin()
        {
            if (CurrentUser == null)
                throw new InvalidOperationException("Please login first.");
        }

        // LOGIN
        public async Task<(bool Success, string Message)> LoginAsync(string email, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                    return (false, "Email and password are required.");

                var user = await _db.GetTableRowAsync<User>("Users", "Email", email);

                if (user == null)
                    return (false, "Email not found");

                if (user.Password != password)
                    return (false, "Incorrect password");

                CurrentUser = user;
                return (true, $"Welcome back, {user.FullName}");
            }
            catch (Exception ex)
            {
                return (false, $"Login failed: {ex.Message}");
            }
        }

        // REGISTER
        public async Task<(bool Success, string Message)> RegisterAsync(User newUser, string confirmPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(newUser.FullName) ||
                    string.IsNullOrWhiteSpace(newUser.Email) ||
                    string.IsNullOrWhiteSpace(newUser.Password))
                {
                    return (false, "All fields are required.");
                }

                if (newUser.Password != confirmPassword)
                    return (false, "Passwords do not match");

                if (newUser.Password.Length < 6)
                    return (false, "Password must be at least 6 characters");

                var existingUser = await _db.GetTableRowAsync<User>("Users", "Email", newUser.Email);
                if (existingUser != null)
                    return (false, "Email already registered");

                newUser.CreatedAt = DateTime.Now;

                var result = await _db.CreateAsync(newUser);
                if (result <= 0)
                    return (false, "User could not be created");

                return (true, "Account created successfully");
            }
            catch (Exception ex)
            {
                return (false, $"Registration failed: {ex.Message}");
            }
        }

        // Update profile for the currently logged-in user
        public async Task<(bool Success, string Message)> UpdateProfileAsync(string fullName)
        {
            try
            {
                if (CurrentUser == null)
                    return (false, "Please login first.");

                if (string.IsNullOrWhiteSpace(fullName))
                    return (false, "Full name cannot be empty.");

                CurrentUser.FullName = fullName;
                var rows = await _db.UpdateAsync(CurrentUser);

                if (rows <= 0)
                    return (false, "Profile was not updated.");

                return (true, "Profile updated.");
            }
            catch (Exception ex)
            {
                return (false, $"Update failed: {ex.Message}");
            }
        }

        // Change password for currently logged-in user
        public async Task<(bool Success, string Message)> ChangePasswordAsync(string currentPassword, string newPassword, string confirmNewPassword)
        {
            try
            {
                if (CurrentUser == null)
                    return (false, "Please login first.");

                if (string.IsNullOrWhiteSpace(currentPassword) ||
                    string.IsNullOrWhiteSpace(newPassword) ||
                    string.IsNullOrWhiteSpace(confirmNewPassword))
                {
                    return (false, "Please fill all password fields.");
                }

                if (CurrentUser.Password != currentPassword)
                    return (false, "Current password is incorrect.");

                if (newPassword != confirmNewPassword)
                    return (false, "Passwords do not match.");

                if (newPassword.Length < 6)
                    return (false, "Password must be at least 6 characters.");

                CurrentUser.Password = newPassword;

                var rows = await _db.UpdateAsync(CurrentUser);
                if (rows <= 0)
                    return (false, "Password was not changed.");

                return (true, "Password changed.");
            }
            catch (Exception ex)
            {
                return (false, $"Password change failed: {ex.Message}");
            }
        }

        // LOGOUT
        public void Logout()
        {
            CurrentUser = null;
        }
    }
}
