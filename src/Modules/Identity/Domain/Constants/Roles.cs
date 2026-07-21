using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIMS_MS.Modules.Identity.Domain.Constants;

public abstract class Roles
{
    public const string Administrator = nameof(Administrator);
    public const string OperatorManager = nameof(OperatorManager);
    public const string ConsultantLogistic = nameof(ConsultantLogistic);
    public const string Guest = nameof(Guest);

    public static readonly IReadOnlyList<string> AllRoles = new List<string>
    {
        Administrator,
        OperatorManager,
        ConsultantLogistic,
        Guest
    };

    public static bool IsValidRole(string role)
    {
        return AllRoles.Contains(role);
    }
}