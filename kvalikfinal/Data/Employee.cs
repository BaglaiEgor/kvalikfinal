using System;
using System.Collections.Generic;

namespace kvalikfinal.Data;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string FullName { get; set; } = null!;

    public string Position { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Email { get; set; }
}
