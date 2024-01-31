using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class PanelManager : MonoBehaviour
{
    [Header("Login")]
    [SerializeField] TMP_InputField log_usernameIField;
    [SerializeField] TMP_InputField log_passwordIField;

    [Header("Register")]
    [SerializeField] TMP_InputField reg_usernameIField;
    [SerializeField] TMP_InputField reg_passwordIField;
    [SerializeField] TMP_InputField cityIField;
    [SerializeField] TMP_InputField addressIField;
    [SerializeField] TMP_InputField f_nameIField;
    [SerializeField] TMP_InputField l_nameIField;
    [SerializeField] BirthdateDropdownManager birthdateDropdownManager;

    [Header("Update Menu")]
    [SerializeField] TMP_InputField UM_passwordIField;
    [SerializeField] TMP_InputField UM_cityIField;
    [SerializeField] TMP_InputField UM_addressIField;
    [SerializeField] TMP_InputField UM_f_nameIField;
    [SerializeField] TMP_InputField UM_l_nameIField;
    [SerializeField] BirthdateDropdownManager UM_birthdateDropdownManager;

    [Header("List New Animal Menu")]
    [SerializeField] TMP_InputField LA_NameIField;
    [SerializeField] TMP_InputField LA_WeightIField;
    [SerializeField] TMP_InputField LA_BreedIField;
    [SerializeField] TMP_Dropdown LA_SexIField;
    [SerializeField] TMP_Dropdown LA_SpeciesIField;
    [SerializeField] BirthdateDropdownManager LA_birthdateDropdownManager;

    [Header("Main Menu Searching")]
    [SerializeField] TMP_Dropdown MS_City_DDown;
    [SerializeField] TMP_Dropdown MS_Pet_DDown;
    [SerializeField] Toggle MS_City_Toggle;
    [SerializeField] Toggle MS_Pet_Toggle;

    [Header("Panels")]
    [SerializeField] GameObject LoginPanel;
    [SerializeField] GameObject RegisterPanel;
    [SerializeField] GameObject MainMenuPanel;
    [SerializeField] GameObject UpdateMenuPanel;
    [SerializeField] GameObject LA_Panel;
    [SerializeField] GameObject Listings_Panel;
    [SerializeField] GameObject Applicants_Panel;
    [SerializeField] GameObject ProductMarket_Panel;
    [SerializeField] GameObject MA_Panel;
    [SerializeField] GameObject MAA_Panel;


    [SerializeField] GameObject board;
    [SerializeField] GameObject animalInventory;
    [SerializeField] GameObject Inventory;
    [SerializeField] GameObject animalsPrefab;
    [SerializeField] GameObject applicantPrefab;
    [SerializeField] GameObject applicantInventory;
    [SerializeField] GameObject productPrefab;
    [SerializeField] GameObject productInventory;
    [SerializeField] GameObject MA_Prefab;
    [SerializeField] GameObject MAA_Prefab;
    [SerializeField] GameObject MA_Inventory;
    [SerializeField] GameObject MAA_Inventory;
    [SerializeField] TextMeshProUGUI MM_UserText;
    [SerializeField] DBManager DBManager;


    List<GameObject> noticeList = new List<GameObject>();
    List<GameObject> animalList = new List<GameObject>();
    List<GameObject> applicantList = new List<GameObject>();
    List<GameObject> productList = new List<GameObject>();
    List<GameObject> MAList = new List<GameObject>();
    List<GameObject> MAAList = new List<GameObject>();
    string Username;
    int UserID;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PanelLogin_LoginButton()
    {
        string text1 = string.Copy(log_usernameIField.text);
        string text2 = string.Copy(log_passwordIField.text);
        bool isLoggedin = false;
        if (!string.IsNullOrEmpty(text1) && !string.IsNullOrEmpty(text2))
            isLoggedin = (bool)DBManager.dbManager.ExecuteQuery(1, DBManager.dbConnection,
                "SELECT * FROM user_info WHERE user_name = @param1 AND user_password = @param2", text1, text2);
        if (isLoggedin)
        {
            LoginPanel.SetActive(false);
            Username = string.Copy(text1);
            UserID = (int)DBManager.dbManager.ExecuteQuery(5, DBManager.dbConnection,
            "SELECT user_id FROM user_info WHERE user_name = @param1", Username);
            SetUpMainMenu();
        }
    }
    public void PanelLogin_RegisterButton()
    {
        LoginPanel.SetActive(false);
        RegisterPanel.SetActive(true);
    }
    public void PanelReg_RegisterButton()
    {
        string text1 = string.Copy(reg_usernameIField.text);
        string text2 = string.Copy(reg_passwordIField.text);
        string text3 = string.Copy(cityIField.text);
        string text4 = string.Copy(addressIField.text);
        string text5 = string.Copy(f_nameIField.text);
        string text6 = string.Copy(l_nameIField.text);

        int selectedDay = int.Parse(birthdateDropdownManager.dayDropdown.options[birthdateDropdownManager.dayDropdown.value].text);
        int selectedMonth = birthdateDropdownManager.monthDropdown.value + 1;
        int selectedYear = int.Parse(birthdateDropdownManager.yearDropdown.options[birthdateDropdownManager.yearDropdown.value].text);
        DateTime birthdate = new DateTime(selectedYear, selectedMonth, selectedDay);
        int id = 0;

        DBManager.dbManager.ExecuteQuery(3, DBManager.dbConnection,
            "INSERT INTO user_info (user_id, user_name, user_password, city, address, f_name, l_name, birth_date) " +
            "VALUES (@param1, @param2, @param3, @param4, @param5, @param6, @param7, @param8)"
           , id, text1, text2, text3, text4, text5, text6, birthdate);

        Username = text1;

        UserID = (int)DBManager.dbManager.ExecuteQuery(5, DBManager.dbConnection,
        "SELECT user_id FROM user_info WHERE user_name = @param1", Username);

        RegisterPanel.SetActive(false);
        SetUpMainMenu();
    }

    void SetUpMainMenu()
    {
        MM_UserText.SetText(Username);
        MainMenuPanel.SetActive(true);

        MS_City_DDown.ClearOptions();
        object cityRes = DBManager.dbManager.ExecuteQuery(10, DBManager.dbConnection,
        "SELECT DISTINCT city FROM notice_board;");
        if (cityRes is IEnumerable<string> citiesEnumerable)
        {
            List<string> citiesList = citiesEnumerable.ToList();
            MS_City_DDown.AddOptions(citiesList);
        }

        MS_Pet_DDown.ClearOptions();
        object petRes = DBManager.dbManager.ExecuteQuery(11, DBManager.dbConnection,
        "SELECT species FROM animal;");
        if (petRes is IEnumerable<string> speciesEnumerable)
        {
            List<string> speciesList = speciesEnumerable.ToList();
            MS_Pet_DDown.AddOptions(speciesList);
        }
        ToggleChanged();
    }

    public void MM_UpdateButton()
    {
        MainMenuPanel.SetActive(false);
        UpdateMenuPanel.SetActive(true);
        object result = DBManager.dbManager.ExecuteQuery(4, DBManager.dbConnection,
        "SELECT * FROM user_info WHERE user_name = @param1", Username);

        if (result != null && result is Tuple<string, string, string, string, string, DateTime>)
        {
            var tupleResult = (Tuple<string, string, string, string, string, DateTime>)result;

            string userPassword = tupleResult.Item1;
            string city = tupleResult.Item2;
            string address = tupleResult.Item3;
            string fName = tupleResult.Item4;
            string lName = tupleResult.Item5;
            DateTime birthDate = tupleResult.Item6;

            UM_passwordIField.text = userPassword;
            UM_cityIField.text = city;
            UM_addressIField.text = address;
            UM_f_nameIField.text = fName;
            UM_l_nameIField.text = lName;

            UM_birthdateDropdownManager.UM_dayDropdown.value = birthDate.Day - 1;
            UM_birthdateDropdownManager.UM_dayDropdown.RefreshShownValue();

            UM_birthdateDropdownManager.UM_monthDropdown.value = birthDate.Month - 1;
            UM_birthdateDropdownManager.UM_monthDropdown.RefreshShownValue();

            int yearIndex = UM_birthdateDropdownManager.UM_yearDropdown.options.FindIndex(option => option.text == birthDate.Year.ToString());
            UM_birthdateDropdownManager.UM_yearDropdown.value = yearIndex;
            UM_birthdateDropdownManager.UM_yearDropdown.RefreshShownValue();
        }
    }

    public void UM_UpdateButton()
    {
        DBManager.dbManager.ExecuteQuery(2, DBManager.dbConnection,
        "UPDATE user_info SET user_password = @param1, city = @param2, address = @param3, " +
        "f_name = @param4, l_name = @param5 WHERE user_name = @param6"
        , UM_passwordIField.text, UM_cityIField.text, UM_addressIField.text
        , UM_f_nameIField.text, UM_l_nameIField.text, Username);
        UpdateMenuPanel.SetActive(false);
        MainMenuPanel.SetActive(true);
    }

    public void MM_ListNewAnimalButton()
    { //LA ekranini olustur.
        MainMenuPanel.SetActive(false);
        LA_Panel.SetActive(true);

        LA_SpeciesIField.ClearOptions();
        object result = DBManager.dbManager.ExecuteQuery(6, DBManager.dbConnection, "SELECT species FROM animal");
        if (result is IEnumerable<string> speciesEnumerable)
        {
            List<string> speciesList = speciesEnumerable.ToList();
            LA_SpeciesIField.AddOptions(speciesList);
        }

    }

    public void LA_ListAnimalButton()
    {
        int notice_id = 0;
        int uid = UserID;

        object res = DBManager.dbManager.ExecuteQuery(7, DBManager.dbConnection, "SELECT animal_id FROM animal WHERE species = @param1", LA_SpeciesIField.options[LA_SpeciesIField.value].text.ToString());
        int aid = Convert.ToInt32(res);
        DateTime notice_date = DateTime.Now;
        string animal_name = string.Copy(LA_NameIField.text);
        int weight = int.Parse(LA_WeightIField.text);
        char selectedSex = LA_SexIField.options[LA_SexIField.value].text[0];
        char sex = char.Parse(selectedSex.ToString());
        string city = (string)DBManager.dbManager.ExecuteQuery(8, DBManager.dbConnection, "SELECT city FROM user_info WHERE user_id = @param1", UserID);
        int selectedDay = int.Parse(birthdateDropdownManager.LA_dayDropdown.options[birthdateDropdownManager.LA_dayDropdown.value].text);
        int selectedMonth = birthdateDropdownManager.LA_monthDropdown.value + 1;
        int selectedYear = int.Parse(birthdateDropdownManager.LA_yearDropdown.options[birthdateDropdownManager.LA_yearDropdown.value].text);
        DateTime birth_date = new DateTime(selectedYear, selectedMonth, selectedDay);
        string breed = string.Copy(LA_BreedIField.text);
        DBManager.dbManager.ExecuteQuery(3, DBManager.dbConnection,
        "INSERT INTO notice_board (notice_id, uid, aid, notice_date, animal_name, weight, sex, city, birth_date, breed) " +
        "VALUES (@param1, @param2, @param3, @param4, @param5, @param6, @param7, @param8, @param9, @param10)"
        , notice_id, uid, aid, notice_date, animal_name, weight, sex, city, birth_date, breed);
        LA_Panel.SetActive(false);
        MainMenuPanel.SetActive(true);
    }

    public void ToggleChanged()
    {
        if(noticeList.Count > 0) 
        { 
            for(int i = 0; i < noticeList.Count; i++)
            {
                Destroy(noticeList[i]);
            }
            noticeList.Clear();
        }

        if (MS_Pet_Toggle.isOn && !MS_City_Toggle.isOn)
        {
            string specie = MS_Pet_DDown.options[MS_Pet_DDown.value].text;
            CalculateAll($"SELECT * FROM searchbyspecies('{specie}','{UserID}')");
        }
        else if(!MS_Pet_Toggle.isOn && MS_City_Toggle.isOn)
        {
            string citie = MS_City_DDown.options[MS_City_DDown.value].text;
            CalculateAll($"SELECT * FROM searchByTown('{citie}','{UserID}')");
        }
        else if(MS_Pet_Toggle.isOn && MS_City_Toggle.isOn)
        {
            string specie = MS_Pet_DDown.options[MS_Pet_DDown.value].text;
            string citie = MS_City_DDown.options[MS_City_DDown.value].text;
            CalculateAll($"SELECT * FROM searchBySpeciesAndTown('{specie}','{citie}','{UserID}')");
        }
        else
        {
            CalculateAll($"SELECT * FROM searchAll('{UserID}')");
        }

        void CalculateAll(string query)
        {
            object result = DBManager.dbManager.ExecuteQuery(9, DBManager.dbConnection, query);
            if (result != null && result is List<Tuple<List<int>, List<string>, DateTime, char, DateTime>>)
            {
                List<Tuple<List<int>, List<string>, DateTime, char, DateTime>> tupleList;
                tupleList = (List<Tuple<List<int>, List<string>, DateTime, char, DateTime>>)result;

                foreach (Tuple<List<int>, List<string>, DateTime, char, DateTime> tuple in tupleList)
                {
                    int notice_id = tuple.Item1[0];
                    int aid = tuple.Item1[1];
                    int weight = tuple.Item1[2];

                    string city = string.Copy(tuple.Item2[0]);
                    string breed = string.Copy(tuple.Item2[1]);
                    string user_name = string.Copy(tuple.Item2[2]);
                    string species = string.Copy(tuple.Item2[3]);
                    string animal_name = string.Copy(tuple.Item2[4]);

                    DateTime notice_date = tuple.Item3;
                    char sex = tuple.Item4;
                    DateTime birth_date = tuple.Item5;

                    GameObject parent = Instantiate(board, Inventory.transform);
                    parent.GetComponent<YourBoard>().PanelManager = this;
                    parent.GetComponent<YourBoard>().id = notice_id;
                    noticeList.Add(parent);

                    parent.transform.Find("Seller (1)").GetComponent<TextMeshProUGUI>().SetText(user_name);
                    parent.transform.Find("City (1)").GetComponent<TextMeshProUGUI>().SetText(city);
                    parent.transform.Find("Weight (1)").GetComponent<TextMeshProUGUI>().SetText(weight.ToString());
                    parent.transform.Find("Sex (1)").GetComponent<TextMeshProUGUI>().SetText(sex.ToString());
                    parent.transform.Find("Species (1)").GetComponent<TextMeshProUGUI>().SetText(species);
                    parent.transform.Find("Animal Name (1)").GetComponent<TextMeshProUGUI>().SetText(animal_name);
                    parent.transform.Find("Breed (1)").GetComponent<TextMeshProUGUI>().SetText(breed);
                    parent.transform.Find("Birth Date (1)").GetComponent<TextMeshProUGUI>().SetText(birth_date.ToShortDateString());
                    parent.transform.Find("Notice Date (1)").GetComponent<TextMeshProUGUI>().SetText(notice_date.ToShortDateString());

                }
            }
        }

    }

    public void MM_MyListingsButton()
    {
        MainMenuPanel.SetActive(false);
        if (animalList.Count > 0)
        {
            for (int i = 0; i < animalList.Count; i++)
            {
                Destroy(animalList[i]);
            }
            animalList.Clear();
        }
        Listings_Panel.SetActive(true);
        List<Tuple<int, DateTime, string, int, char, string, DateTime, Tuple<string>>> resultList;
        object res = DBManager.dbManager.ExecuteQuery(12, DBManager.dbConnection, $"SELECT * FROM listmynotices('{UserID}')");

        resultList = (List<Tuple<int, DateTime, string, int, char, string, DateTime, Tuple<string>>>)res;

        foreach(var result in resultList)
        {
            GameObject parent = Instantiate(animalsPrefab, animalInventory.transform);
            parent.GetComponent<YourAnimal>().PanelManager = this;
            parent.GetComponent<YourAnimal>().id = result.Item1;
            animalList.Add(parent);
            parent.transform.Find("notice_id (1)").GetComponent<TextMeshProUGUI>().SetText(result.Item1.ToString());
            parent.transform.Find("notice_date (1)").GetComponent<TextMeshProUGUI>().SetText(result.Item2.ToShortDateString());
            parent.transform.Find("animal_name (1)").GetComponent<TextMeshProUGUI>().SetText(result.Item3.ToString());
            parent.transform.Find("weight (1)").GetComponent<TextMeshProUGUI>().SetText(result.Item4.ToString());
            parent.transform.Find("Sex (1)").GetComponent<TextMeshProUGUI>().SetText(result.Item5.ToString());
            parent.transform.Find("city (1)").GetComponent<TextMeshProUGUI>().SetText(result.Item6.ToString());
            parent.transform.Find("birthdate (1)").GetComponent<TextMeshProUGUI>().SetText(result.Item7.ToShortDateString());
            parent.transform.Find("breed (1)").GetComponent<TextMeshProUGUI>().SetText(result.Rest.Item1.ToString());
        }
    }

    public void MM_ProductMarketButton()
    {
        MainMenuPanel.SetActive(false);
        productList.Clear();
        ProductMarket_Panel.SetActive(true);
        object result = DBManager.dbManager.ExecuteQuery(14, DBManager.dbConnection, "SELECT * FROM listAllProducts()");
        List<Tuple<int, string, int, int, DateTime, string, int>> products = (List<Tuple<int, string, int, int, DateTime, string, int>>) result;
        foreach(Tuple<int, string, int, int, DateTime, string, int> product in products)
        {
            GameObject parent = Instantiate(productPrefab, productInventory.transform);
            productList.Add(parent);
            parent.GetComponent<YourProduct>().PanelManager = this; parent.GetComponent<YourProduct>().id = product.Item1;
            parent.transform.Find("Product id (1)").GetComponent<TextMeshProUGUI>().SetText(product.Item1.ToString());
            parent.transform.Find("Product name (1)").GetComponent<TextMeshProUGUI>().SetText(product.Item2);
            parent.transform.Find("price (1)").GetComponent<TextMeshProUGUI>().SetText(product.Item3.ToString());
            parent.transform.Find("stock (1)").GetComponent<TextMeshProUGUI>().SetText(product.Item4.ToString());
            parent.transform.Find("upload date (1)").GetComponent<TextMeshProUGUI>().SetText(product.Item5.ToShortDateString().ToString());
            parent.transform.Find("Species (1)").GetComponent<TextMeshProUGUI>().SetText(product.Item6);
        }
    }
    public void MainShowOtherApplicants(int id)
    {
        MA_Panel.SetActive(false);
        MAA_Panel.SetActive(true);
        object res = DBManager.dbManager.ExecuteQuery(13, DBManager.dbConnection, $"SELECT * FROM applicantlist('{id}')");
        List<Tuple<string, string, string, string>> applicants = (List<Tuple<string, string, string, string>>)res;
        foreach (var applicant in applicants)
        {
            GameObject parent = Instantiate(MAA_Prefab, MAA_Inventory.transform);
            MAAList.Add(parent);
            parent.GetComponent<YourApplicant>().PanelManager = this; parent.GetComponent<YourApplicant>().id = id;
            parent.transform.Find("City (1)").GetComponent<TextMeshProUGUI>().SetText(applicant.Item1);
            parent.transform.Find("f_name (1)").GetComponent<TextMeshProUGUI>().SetText(applicant.Item2);
            parent.transform.Find("l_name (1)").GetComponent<TextMeshProUGUI>().SetText(applicant.Item3);
            parent.transform.Find("user_name (1)").GetComponent<TextMeshProUGUI>().SetText(applicant.Item4);
        }

    }
    public void MM_MyApplicationsButton()
    {
        MainMenuPanel.SetActive(false);
        MA_Panel.SetActive(true);
        MAList.Clear();
        object res = DBManager.dbManager.ExecuteQuery(12, DBManager.dbConnection, $"SELECT * FROM listMyApplications('{UserID}')");

        List<Tuple<int, DateTime, string, int, char, string, DateTime, Tuple<string>>> resultList = 
            (List<Tuple<int, DateTime, string, int, char, string, DateTime, Tuple<string>>>)res;

        foreach (var result in resultList)
        {
            GameObject parent = Instantiate(MA_Prefab, MA_Inventory.transform);
            parent.GetComponent<YourApplication>().PanelManager = this;
            parent.GetComponent<YourApplication>().id = result.Item1;
            MAList.Add(parent);
            parent.transform.Find("notice_id (1)").GetComponent<TextMeshProUGUI>().SetText(result.Item1.ToString());
            parent.transform.Find("notice_date (1)").GetComponent<TextMeshProUGUI>().SetText(result.Item2.ToShortDateString());
            parent.transform.Find("animal_name (1)").GetComponent<TextMeshProUGUI>().SetText(result.Item3.ToString());
            parent.transform.Find("weight (1)").GetComponent<TextMeshProUGUI>().SetText(result.Item4.ToString());
            parent.transform.Find("Sex (1)").GetComponent<TextMeshProUGUI>().SetText(result.Item5.ToString());
            parent.transform.Find("city (1)").GetComponent<TextMeshProUGUI>().SetText(result.Item6.ToString());
            parent.transform.Find("birthdate (1)").GetComponent<TextMeshProUGUI>().SetText(result.Item7.ToShortDateString());
            parent.transform.Find("breed (1)").GetComponent<TextMeshProUGUI>().SetText(result.Rest.Item1.ToString());
        }

    }
    public void MainBuyButton(int id)
    {
        if (productList.Count > 0)
        {
            for (int i = 0; i < productList.Count; i++)
            {
                Destroy(productList[i]);
            }
            productList.Clear();
        }
        DBManager.dbManager.ExecuteQuery(0, DBManager.dbConnection, "DELETE FROM products WHERE product_id = @param1", id);
        MM_ProductMarketButton();
    }
    public void MainAdoptAnimal(int noticeID)
    {
        DBManager.dbManager.ExecuteQuery(0, DBManager.dbConnection, "INSERT INTO application VALUES(@param1,@param2)", noticeID,UserID);
        ToggleChanged();
    }

    public void MainShowApplicant(int id)
    {
        Listings_Panel.SetActive(false);
        Applicants_Panel.SetActive(true);
        List<Tuple<string, string ,string ,string>> applicants = new();
        object res = DBManager.dbManager.ExecuteQuery(13, DBManager.dbConnection, $"SELECT * FROM applicantlist('{id}')");
        applicants = (List<Tuple<string, string, string, string>>)res;
        foreach (var applicant in applicants)
        {
            GameObject parent = Instantiate(applicantPrefab, applicantInventory.transform);
            applicantList.Add(parent);
            parent.GetComponent<YourApplicant>().PanelManager = this; parent.GetComponent<YourApplicant>().id = id;
            parent.transform.Find("City (1)").GetComponent<TextMeshProUGUI>().SetText(applicant.Item1);
            parent.transform.Find("f_name (1)").GetComponent<TextMeshProUGUI>().SetText(applicant.Item2);
            parent.transform.Find("l_name (1)").GetComponent<TextMeshProUGUI>().SetText(applicant.Item3);
            parent.transform.Find("user_name (1)").GetComponent<TextMeshProUGUI>().SetText(applicant.Item4);
        }
    }

    public void MainAcceptApplicant(int id)
    {
        Applicants_Panel.SetActive(false);
        if (applicantList.Count > 0)
        {
            for (int i = 0; i < applicantList.Count; i++)
            {
                Destroy(applicantList[i]);
            }
            applicantList.Clear();
        }
        MainMenuPanel.SetActive(true); 
        DBManager.dbManager.ExecuteQuery(0, DBManager.dbConnection, "DELETE FROM notice_board WHERE notice_id = @param1", id);
    }
    public void DeleteButton(int id)
    {
        DBManager.dbManager.ExecuteQuery(0, DBManager.dbConnection, "DELETE FROM notice_board WHERE notice_id = @param1", id);
    }

    public void QuitButton()
    {
        Application.Quit();
    }
    public void RP_BackButton() { RegisterPanel.SetActive(false); LoginPanel.SetActive(true); }
    public void MM_BackButton() { MainMenuPanel.SetActive(false); LoginPanel.SetActive(true); }
    public void UM_BackButton() { UpdateMenuPanel.SetActive(false); SetUpMainMenu(); }
    public void LA_BackButton() { LA_Panel.SetActive(false); SetUpMainMenu(); }
    public void ML_BackButton() { Listings_Panel.SetActive(false); SetUpMainMenu(); }
    public void AP_BackButton() { Applicants_Panel.SetActive(false); Listings_Panel.SetActive(true); }
    public void MA_BackButton() 
    { 
        MA_Panel.SetActive(false); 
        SetUpMainMenu();
        if (MAList.Count > 0)
        {
            for (int i = 0; i < MAList.Count; i++)
            {
                Destroy(MAList[i]);
            }
            MAList.Clear();
        }
    }
    public void PM_BackButton() 
    { 
        ProductMarket_Panel.SetActive(false); 
        if (productList.Count > 0)
        {
            for (int i = 0; i < productList.Count; i++)
            {
                Destroy(productList[i]);
            }
            productList.Clear();
        }
        SetUpMainMenu();
    }
    public void MAA_BackButton() 
    { 
        MAA_Panel.SetActive(false); 
        MA_Panel.SetActive(true); 
        if (MAAList.Count > 0)
        {
            for (int i = 0; i < MAAList.Count; i++)
            {
                Destroy(MAAList[i]);
            }
            MAAList.Clear();
        }
    }

    

}