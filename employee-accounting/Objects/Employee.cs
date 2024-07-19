using System;
using System.Collections.Generic;

namespace employee_accounting.Objects;

public partial class Employee
{
    public int Id { get; set; }

    public DateOnly BirthDate { get; set; }

    public string Name { get; set; } = null!;

    public int Sex { get; set; }

    public int GetAge()
    {
        return DateTime.Now.Year - BirthDate.Year;
    }
}
