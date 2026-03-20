using NNPensionPlanner.Models;

namespace NNPensionPlanner.Data;

public static class SeedData
{
    public static void Initialize(
        InMemoryRepository<Participant> participants,
        InMemoryRepository<PensionPlan> plans,
        InMemoryRepository<Enrollment> enrollments,
        InMemoryRepository<Contribution> contributions)
    {
        // --- Pension Plans ---
        var basisPlan = new PensionPlan
        {
            Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000001"),
            Name = "NN Basis Pensioen",
            Description = "Our standard defined contribution pension plan with employer matching up to 4%.",
            Type = PlanType.DefinedContribution,
            EmployerMatchPercentage = 4.0m,
            MaxContributionPercentage = 8.0m,
            VestingPeriodYears = 2,
            MinimumAnnualSalary = 25000m,
            IsActive = true
        };

        var flexPlan = new PensionPlan
        {
            Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000002"),
            Name = "NN Flex Pensioen",
            Description = "Flexible pension plan allowing higher voluntary contributions with 6% employer match.",
            Type = PlanType.DefinedContribution,
            EmployerMatchPercentage = 6.0m,
            MaxContributionPercentage = 12.0m,
            VestingPeriodYears = 1,
            MinimumAnnualSalary = 35000m,
            IsActive = true
        };

        var premiumPlan = new PensionPlan
        {
            Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000003"),
            Name = "NN Premium Pensioen",
            Description = "Premium defined benefit plan guaranteeing a percentage of final salary upon retirement.",
            Type = PlanType.DefinedBenefit,
            EmployerMatchPercentage = 8.0m,
            MaxContributionPercentage = 15.0m,
            VestingPeriodYears = 3,
            MinimumAnnualSalary = 50000m,
            IsActive = true
        };

        var starterPlan = new PensionPlan
        {
            Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000004"),
            Name = "NN Starter Pensioen",
            Description = "Entry-level pension plan designed for young professionals starting their career.",
            Type = PlanType.DefinedContribution,
            EmployerMatchPercentage = 3.0m,
            MaxContributionPercentage = 6.0m,
            VestingPeriodYears = 1,
            MinimumAnnualSalary = 20000m,
            IsActive = true
        };

        plans.Seed(new[] { basisPlan, flexPlan, premiumPlan, starterPlan });

        // --- Participants ---
        var jan = new Participant
        {
            Id = Guid.Parse("b2c3d4e5-0002-0002-0002-000000000001"),
            FirstName = "Jan",
            LastName = "de Vries",
            DateOfBirth = new DateTime(1985, 3, 15),
            Email = "jan.devries@example.nl",
            EmployerName = "TechCorp BV",
            AnnualSalary = 65000m,
            JoinDate = new DateTime(2020, 1, 15)
        };

        var emma = new Participant
        {
            Id = Guid.Parse("b2c3d4e5-0002-0002-0002-000000000002"),
            FirstName = "Emma",
            LastName = "Bakker",
            DateOfBirth = new DateTime(1990, 7, 22),
            Email = "emma.bakker@example.nl",
            EmployerName = "FinanceHuis NV",
            AnnualSalary = 72000m,
            JoinDate = new DateTime(2021, 6, 1)
        };

        var pieter = new Participant
        {
            Id = Guid.Parse("b2c3d4e5-0002-0002-0002-000000000003"),
            FirstName = "Pieter",
            LastName = "van den Berg",
            DateOfBirth = new DateTime(1978, 11, 8),
            Email = "pieter.vandenberg@example.nl",
            EmployerName = "Bouwgroep Nederland",
            AnnualSalary = 58000m,
            JoinDate = new DateTime(2018, 9, 1)
        };

        var sophie = new Participant
        {
            Id = Guid.Parse("b2c3d4e5-0002-0002-0002-000000000004"),
            FirstName = "Sophie",
            LastName = "Jansen",
            DateOfBirth = new DateTime(1995, 5, 30),
            Email = "sophie.jansen@example.nl",
            EmployerName = "DataFlow BV",
            AnnualSalary = 48000m,
            JoinDate = new DateTime(2023, 2, 1)
        };

        var willem = new Participant
        {
            Id = Guid.Parse("b2c3d4e5-0002-0002-0002-000000000005"),
            FirstName = "Willem",
            LastName = "Mulder",
            DateOfBirth = new DateTime(1970, 9, 12),
            Email = "willem.mulder@example.nl",
            EmployerName = "LogistiekPartners BV",
            AnnualSalary = 82000m,
            JoinDate = new DateTime(2015, 4, 1)
        };

        participants.Seed(new[] { jan, emma, pieter, sophie, willem });

        // --- Enrollments ---
        var janEnrollment = new Enrollment
        {
            Id = Guid.Parse("c3d4e5f6-0003-0003-0003-000000000001"),
            ParticipantId = jan.Id,
            PlanId = flexPlan.Id,
            StartDate = new DateTime(2020, 2, 1),
            Status = EnrollmentStatus.Active,
            ContributionPercentage = 5.0m
        };

        var emmaEnrollment = new Enrollment
        {
            Id = Guid.Parse("c3d4e5f6-0003-0003-0003-000000000002"),
            ParticipantId = emma.Id,
            PlanId = premiumPlan.Id,
            StartDate = new DateTime(2021, 7, 1),
            Status = EnrollmentStatus.Active,
            ContributionPercentage = 7.0m
        };

        var pieterEnrollment = new Enrollment
        {
            Id = Guid.Parse("c3d4e5f6-0003-0003-0003-000000000003"),
            ParticipantId = pieter.Id,
            PlanId = basisPlan.Id,
            StartDate = new DateTime(2018, 10, 1),
            Status = EnrollmentStatus.Active,
            ContributionPercentage = 4.0m
        };

        var sophieEnrollment = new Enrollment
        {
            Id = Guid.Parse("c3d4e5f6-0003-0003-0003-000000000004"),
            ParticipantId = sophie.Id,
            PlanId = starterPlan.Id,
            StartDate = new DateTime(2023, 3, 1),
            Status = EnrollmentStatus.Active,
            ContributionPercentage = 3.0m
        };

        var willemEnrollment = new Enrollment
        {
            Id = Guid.Parse("c3d4e5f6-0003-0003-0003-000000000005"),
            ParticipantId = willem.Id,
            PlanId = premiumPlan.Id,
            StartDate = new DateTime(2015, 5, 1),
            Status = EnrollmentStatus.Active,
            ContributionPercentage = 10.0m
        };

        enrollments.Seed(new[] { janEnrollment, emmaEnrollment, pieterEnrollment, sophieEnrollment, willemEnrollment });

        // --- Contributions (last 6 months for each enrollment) ---
        var allEnrollments = new[]
        {
            (janEnrollment, jan, flexPlan),
            (emmaEnrollment, emma, premiumPlan),
            (pieterEnrollment, pieter, basisPlan),
            (sophieEnrollment, sophie, starterPlan),
            (willemEnrollment, willem, premiumPlan)
        };

        var seedContributions = new List<Contribution>();
        foreach (var (enrollment, participant, plan) in allEnrollments)
        {
            for (int monthsAgo = 5; monthsAgo >= 0; monthsAgo--)
            {
                var date = DateTime.UtcNow.AddMonths(-monthsAgo);
                var monthlyGross = participant.AnnualSalary / 12;
                var employeeAmount = monthlyGross * (enrollment.ContributionPercentage / 100);
                var employerAmount = monthlyGross * (Math.Min(enrollment.ContributionPercentage, plan.EmployerMatchPercentage) / 100);

                seedContributions.Add(new Contribution
                {
                    Id = Guid.NewGuid(),
                    EnrollmentId = enrollment.Id,
                    Date = new DateTime(date.Year, date.Month, 1),
                    EmployeeAmount = Math.Round(employeeAmount, 2),
                    EmployerAmount = Math.Round(employerAmount, 2),
                    Type = ContributionType.Regular
                });
            }
        }

        contributions.Seed(seedContributions);
    }
}
