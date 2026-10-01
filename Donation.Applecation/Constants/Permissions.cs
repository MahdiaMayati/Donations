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

    public static class Donors
    {
        public const string View = "Permissions.Donors.View";
        public const string Create = "Permissions.Donors.Create";
        public const string Edit = "Permissions.Donors.Edit";
        public const string Delete = "Permissions.Donors.Delete";
    }

    public static class Beneficiaries
    {
        public const string View = "Permissions.Beneficiaries.View";
        public const string Create = "Permissions.Beneficiaries.Create";
        public const string Edit = "Permissions.Beneficiaries.Edit";
        public const string Delete = "Permissions.Beneficiaries.Delete";
    }

    public static class FamilyMembers
    {
        public const string View = "Permissions.FamilyMembers.View";
        public const string Create = "Permissions.FamilyMembers.Create";
        public const string Edit = "Permissions.FamilyMembers.Edit";
        public const string Delete = "Permissions.FamilyMembers.Delete";
    }

    public static class Volunteers
    {
        public const string View = "Permissions.Volunteers.View";
        public const string Create = "Permissions.Volunteers.Create";
        public const string Edit = "Permissions.Volunteers.Edit";
        public const string Delete = "Permissions.Volunteers.Delete";
    }

    public static class Warehouses
    {
        public const string View = "Permissions.Warehouses.View";
        public const string Create = "Permissions.Warehouses.Create";
        public const string Edit = "Permissions.Warehouses.Edit";
        public const string Delete = "Permissions.Warehouses.Delete";
    }

    public static class StorageLocations
    {
        public const string View = "Permissions.StorageLocations.View";
        public const string Create = "Permissions.StorageLocations.Create";
        public const string Edit = "Permissions.StorageLocations.Edit";
        public const string Delete = "Permissions.StorageLocations.Delete";
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
            Organizations.Delete,
            Donors.View,
            Donors.Create,
            Donors.Edit,
            Donors.Delete,
            Beneficiaries.View,
            Beneficiaries.Create,
            Beneficiaries.Edit,
            Beneficiaries.Delete,
            FamilyMembers.View,
            FamilyMembers.Create,
            FamilyMembers.Edit,
            FamilyMembers.Delete,
            Volunteers.View,
            Volunteers.Create,
            Volunteers.Edit,
            Volunteers.Delete,
            Warehouses.View,
            Warehouses.Create,
            Warehouses.Edit,
            Warehouses.Delete,
            StorageLocations.View,
            StorageLocations.Create,
            StorageLocations.Edit,
            StorageLocations.Delete
        };
}
