namespace Donation.Application.Constants;

public static class Permissions
{
    public static class Donations
    {
        public const string View = "Permissions.Donations.View";
        public const string Create = "Permissions.Donations.Create";
        public const string Edit = "Permissions.Donations.Edit";
        public const string Delete = "Permissions.Donations.Delete";
    }

    public static class Users
    {
        public const string View = "Permissions.Users.View";
        public const string Create = "Permissions.Users.Create";
        public const string Edit = "Permissions.Users.Edit";
        public const string Delete = "Permissions.Users.Delete";
    }

    // دالة أو خاصية لتجميع كل الصلاحيات الموجودة تلقائياً
    public static List<string> AllPermissionsList =>
        new List<string>
        {
            Donations.View,
            Donations.Create,
            Donations.Edit,
            Donations.Delete,
            Users.View,
            Users.Create,
            Users.Edit,
            Users.Delete
        };
}