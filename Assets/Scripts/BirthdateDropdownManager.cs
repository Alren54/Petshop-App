using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BirthdateDropdownManager : MonoBehaviour
{
    public TMP_Dropdown dayDropdown;
    public TMP_Dropdown monthDropdown;
    public TMP_Dropdown yearDropdown;

    public TMP_Dropdown UM_dayDropdown;
    public TMP_Dropdown UM_monthDropdown;
    public TMP_Dropdown UM_yearDropdown;

    public TMP_Dropdown LA_dayDropdown;
    public TMP_Dropdown LA_monthDropdown;
    public TMP_Dropdown LA_yearDropdown;

    void Start()
    {
        dayDropdown.ClearOptions();
        monthDropdown.ClearOptions();
        yearDropdown.ClearOptions();

        UM_dayDropdown.ClearOptions();
        UM_monthDropdown.ClearOptions();
        UM_yearDropdown.ClearOptions();

        LA_dayDropdown.ClearOptions();
        LA_monthDropdown.ClearOptions();
        LA_yearDropdown.ClearOptions();

        InitializeDropdownOptions();
    }

    void InitializeDropdownOptions()
    {
        List<string> dayOptions = new List<string>();
        for (int i = 1; i <= 31; i++)
        {
            dayOptions.Add(i.ToString());
        }
        dayDropdown.AddOptions(dayOptions);

        List<string> monthOptions = new List<string>();
        for (int i = 1; i <= 12; i++)
        {
            monthOptions.Add(i.ToString());
        }
        monthDropdown.AddOptions(monthOptions);

        List<string> yearOptions = new List<string>();
        int currentYear = System.DateTime.Now.Year;
        for (int i = currentYear; i >= currentYear - 100; i--)
        {
            yearOptions.Add(i.ToString());
        }
        yearDropdown.AddOptions(yearOptions);



        List<string> UM_dayOptions = new List<string>();
        for (int i = 1; i <= 31; i++)
        {
            UM_dayOptions.Add(i.ToString());
        }
        UM_dayDropdown.AddOptions(UM_dayOptions);

        List<string> UM_monthOptions = new List<string>();
        for (int i = 1; i <= 12; i++)
        {
            UM_monthOptions.Add(i.ToString());
        }
        UM_monthDropdown.AddOptions(UM_monthOptions);

        List<string> UM_yearOptions = new List<string>();
        int UM_currentYear = System.DateTime.Now.Year;
        for (int i = UM_currentYear; i >= UM_currentYear - 100; i--)
        {
            UM_yearOptions.Add(i.ToString());
        }
        UM_yearDropdown.AddOptions(UM_yearOptions);

        List<string> LA_dayOptions = new List<string>();
        for (int i = 1; i <= 31; i++)
        {
            LA_dayOptions.Add(i.ToString());
        }
        LA_dayDropdown.AddOptions(LA_dayOptions);

        List<string> LA_monthOptions = new List<string>();
        for (int i = 1; i <= 12; i++)
        {
            LA_monthOptions.Add(i.ToString());
        }
        LA_monthDropdown.AddOptions(LA_monthOptions);

        List<string> LA_yearOptions = new List<string>();
        int LA_currentYear = System.DateTime.Now.Year;
        for (int i = LA_currentYear; i >= LA_currentYear - 100; i--)
        {
            LA_yearOptions.Add(i.ToString());
        }
        LA_yearDropdown.AddOptions(LA_yearOptions);
    }
}
