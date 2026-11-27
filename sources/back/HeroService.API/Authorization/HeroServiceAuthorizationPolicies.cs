namespace HeroService.API.Authorization;

public static class HeroServiceAuthorizationPolicies
{
    public const string CatalogueRead = "HeroCatalogue.Read";
    public const string CatalogueWrite = "HeroCatalogue.Write";
    public const string ReleaseManagement = "HeroCatalogue.ReleaseManagement";

    public const string DefaultReleaseManagementRole = ReleaseManagement;
}
