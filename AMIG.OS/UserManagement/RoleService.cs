namespace AMIG.OS.UserSystemManagement
{
    public class RoleService
    {
        // Prüfen, ob der Benutzer Admin ist
        public bool IsAdmin(User user)
        {
            return user.Role == "Admin";
        }

        // Prüfen, ob der Benutzer ein Standard-Benutzer ist
        public bool IsStandardUser(User user)
        {
            return user.Role == "Standard";
        }

        // Ändern der Benutzerrolle
        public void AssignRole(User user, string newRole)
        {
            user.ChangeRole(newRole);
        }
    }
}

