using System.Linq;
using System.Numerics;
using Content.Shared.AdventureSpace.Roles.CCO;
using Content.Shared.AdventureSpace.Roles.Salary;
using Content.Shared.StationRecords;
using Content.Shared.StatusIcon;
using Robust.Client.UserInterface.Controls;
using SecretCCVars = Content.Shared.AdventureSpace.CCVars.SecretCCVars;

namespace Content.Client.AdventureSpace.Roles.CCO.Console;

public sealed partial class CcoConsoleWindow
{
    private KeyValuePair<uint, GeneralStationRecord>? _selectedCrewMemberRecord;

    private void SetupSalaries()
    {
        SalaryCount.OnTextChanged += OnSalaryTextChanged;
        CrewMemberBonusButton.OnPressed += _ => CrewMemberApplySalaryBonus();
        CrewMemberPenaltyButton.OnPressed += _ => CrewMemberApplySalaryPenalty();
    }

    private void CrewMemberApplySalaryBonus()
    {
        if (_selectedCrewMemberRecord is not { } record)
            return;

        var bonus = GetSalaryCountInput();
        _bui.CrewMemberApplySalaryBonus(bonus, record.Key);
    }

    private void CrewMemberApplySalaryPenalty()
    {
        if (_selectedCrewMemberRecord is not { } record)
            return;

        var penalty = GetSalaryCountInput();
        _bui.CrewMemberApplySalaryPenalty(penalty, record.Key);
    }

    private int GetSalaryCountInput()
    {
        return int.Parse(SalaryCount.Text);
    }

    private void OnSalaryTextChanged(LineEdit.LineEditEventArgs args)
    {
        var isSalaryCorrect = int.TryParse(args.Text, out var salary) && salary > 0;

        CrewMemberBonusButton.Disabled =
            !isSalaryCorrect || _selectedCrewMemberRecord?.Value.Salary?.SalaryBonuses.Count != 0;
        CrewMemberPenaltyButton.Disabled = !isSalaryCorrect;
    }

    private void BindSalaries(CcoConsoleSalaries salaries)
    {
        CrewMembersList.DisposeAllChildren();
        CrewMembersList.RemoveAllChildren();

        if (salaries.Records == null || !_cfg.GetCVar(SecretCCVars.EconomyEnabled))
            return;

        foreach (var record in salaries.Records)
        {
            var crewButton = GetCrewContainer(record);
            CrewMembersList.AddChild(crewButton);
        }
    }

    private void BindSelectedUserSalary(string userName,
        string jobName,
        KeyValuePair<uint, GeneralStationRecord>? record)
    {
        _selectedCrewMemberRecord = record;

        CrewMemberName.Text = userName;
        CrewMemberJob.Text = jobName;

        if (record?.Value.Salary is not { } salary)
        {
            CrewMemberSalary.Text = Loc.GetString("cco-console-no-salary");

            CrewMemberPenaltiesContainer.Visible = false;
            CrewMemberBonusesContainer.Visible = false;

            return;
        }

        CrewMemberSalary.Text = Loc.GetString("cco-console-salary", ("credits", salary.Salary));

        BindSalaryPenalties(salary.SalaryPenalties);
        BindSalaryBonuses(salary.SalaryBonuses);

        if (salary.SalaryBonuses.Count == 0)
            return;

        CrewMemberBonusButton.Disabled = true;
        CrewMemberBonusButton.ToolTip = Loc.GetString("cco-console-max-bonus-tooltip");
    }

    private void BindSalaryPenalties(List<CrewSalaryPenalty> penalties)
    {
        if (penalties.Count == 0)
        {
            CrewMemberPenaltiesContainer.Visible = false;
            return;
        }

        CrewMemberPenaltiesContainer.Visible = true;
        CrewMemberPenaltiesSum.Text = Loc.GetString("cco-console-bonus-or-penalty",
            ("credits", penalties.Sum(penalty => penalty.Penalty)),
            ("count", penalties.Count));
    }


    private void BindSalaryBonuses(List<CrewSalaryBonus> bonuses)
    {
        if (bonuses.Count == 0)
        {
            CrewMemberBonusesContainer.Visible = false;
            return;
        }

        CrewMemberBonusesContainer.Visible = true;
        CrewMemberBonusesSum.Text = Loc.GetString("cco-console-bonus-or-penalty",
            ("credits", bonuses.Sum(bonus => bonus.Bonus)),
            ("count", bonuses.Count));
    }

    private ContainerButton GetCrewContainer(KeyValuePair<uint, GeneralStationRecord> record)
    {
        var button = new Button
        {
            HorizontalExpand = true,
        };

        var jobContainer = new BoxContainer
        {
            Orientation = BoxContainer.LayoutOrientation.Horizontal,
            HorizontalExpand = true,
        };

        if (_prototypeManager.TryIndex<StatusIconPrototype>(record.Value.JobIcon, out var proto))
        {
            var jobIcon = new TextureRect
            {
                TextureScale = new Vector2(2f, 2f),
                Stretch = TextureRect.StretchMode.KeepCentered,
                Texture = _spriteSystem.Frame0(proto.Icon),
                Margin = new Thickness(5, 0, 5, 0),
            };

            jobContainer.AddChild(jobIcon);
        }

        var jobLabel = new Label
        {
            Text = record.Value.JobTitle,
            HorizontalExpand = true,
            ClipText = true,
        };

        var nameLabel = new Label
        {
            Text = record.Value.Name,
            HorizontalExpand = true,
            ClipText = true,
        };

        jobContainer.AddChild(jobLabel);
        jobContainer.AddChild(nameLabel);

        button.AddChild(jobContainer);
        button.OnPressed += _ =>
        {
            CrewMemberSalaryContainer.Visible = true;
            BindSelectedUserSalary(record.Value.Name, record.Value.JobTitle, record);
        };

        if (_selectedCrewMemberRecord is not { } selectedRecord)
            return button;

        if (record.Key == selectedRecord.Key)
            BindSelectedUserSalary(record.Value.Name, record.Value.JobTitle, record);

        return button;
    }
}
