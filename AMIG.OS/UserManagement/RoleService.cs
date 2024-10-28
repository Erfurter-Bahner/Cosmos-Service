namespace AMIG.OS.UserSystemManagement
{
    public class RoleService
    {
        // Prüfen, ob der Benutzer Admin ist
        public bool IsAdmin(User user)
        {
            return user.Role == UserRole.Admin;
        }

        // Prüfen, ob der Benutzer ein Standard-Benutzer ist
        public bool IsStandardUser(User user)
        {
            return user.Role == UserRole.Standard;
        }

        // Ändern der Benutzerrolle
        public void AssignRole(User user, UserRole newRole)
        {
            user.ChangeRole(newRole);
        }
    }
}

