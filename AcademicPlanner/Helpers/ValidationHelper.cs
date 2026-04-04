using AcademicPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AcademicPlanner.Helpers;

public static class ValidationHelper
{
    public static bool IsRequired(string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    public static bool DatesAreValid(DateTime start, DateTime end)
    {
        return start <= end;
    }

    public static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return Regex.IsMatch(
            email.Trim(),
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.IgnoreCase);
    }
    public static bool CanAddAnotherAssessment(List<Assessment> assessments)
    {
        return assessments.Count < 2;
    }

    public static bool HasDuplicateAssessmentType(
        List<Assessment> assessments,
        string type,
        int currentAssessmentId = 0)
    {
        return assessments.Any(a =>
            a.Type.Equals(type, StringComparison.OrdinalIgnoreCase) &&
            a.Id != currentAssessmentId);
    }
}