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

    public static class Cities
    {
        public const string View = "Permissions.Cities.View";
        public const string Create = "Permissions.Cities.Create";
        public const string Edit = "Permissions.Cities.Edit";
        public const string Delete = "Permissions.Cities.Delete";
    }

    public static class Areas
    {
        public const string View = "Permissions.Areas.View";
        public const string Create = "Permissions.Areas.Create";
        public const string Edit = "Permissions.Areas.Edit";
        public const string Delete = "Permissions.Areas.Delete";
    }

    public static class Addresses
    {
        public const string View = "Permissions.Addresses.View";
        public const string Create = "Permissions.Addresses.Create";
        public const string Edit = "Permissions.Addresses.Edit";
        public const string Delete = "Permissions.Addresses.Delete";
    }

    public static class Organizations
    {
        public const string View = "Permissions.Organizations.View";
        public const string Create = "Permissions.Organizations.Create";
        public const string Edit = "Permissions.Organizations.Edit";
        public const string Delete = "Permissions.Organizations.Delete";
    }

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
            Users.Delete,
            Cities.View,
            Cities.Create,
            Cities.Edit,
            Cities.Delete,
            Areas.View,
            Areas.Create,
            Areas.Edit,
            Areas.Delete,
            Addresses.View,
            Addresses.Create,
            Addresses.Edit,
            Addresses.Delete,
            Organizations.View,
            Organizations.Create,
            Organizations.Edit,
            Organizations.Delete
        };
}
