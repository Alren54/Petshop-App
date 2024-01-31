using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Npgsql;
using System;
using TMPro;

public class DatabaseManager
{
    private const string connectionString = "Host=127.0.0.1:8086;Username=postgres;Password=123456;Database=UnityDB;";
    private GameObject ErrorBox;
    public NpgsqlConnection Connect()
    {
        ErrorBox = GameObject.Find("Canvas/ErrorBox");
        ErrorBox.SetActive(false);
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        try
        {
            connection.Open();
            Debug.Log("Connected to PostgreSQL");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error: {ex.Message}");
        }

        return connection;
    }

    public object ExecuteQuery(int todo, NpgsqlConnection connection, string query, params object[] parameters)
    {
        try
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, connection))
            {
                if (parameters != null && parameters.Length > 0)
                {
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        cmd.Parameters.AddWithValue($"@param{i + 1}", parameters[i]);
                    }
                }
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    List<Tuple<int, string, int, int, DateTime, string, int>> allProducts = new();
                    List<string> specieList = new();
                    List<Tuple<List<int>, List<string>, DateTime, char, DateTime>> ilanList = new();
                    List<string> allCities = new();
                    List<string> allSpecies = new();
                    List<Tuple<int, DateTime, string, int, char, string, DateTime, Tuple<string>>> allAnimals = new();
                    List<Tuple<string, string, string, string>> allApplicants = new();
                    while (reader.Read())
                    {
                        switch (todo)
                        {
                            case 0:
                                return null;
                            case 1:
                                return true;
                            case 2:
                                break;
                            case 3:
                                Debug.Log("inserted");
                                return true;
                            case 4:
                                Tuple<string, string, string, string, string, DateTime> a;
                                {
                                    string userPassword = reader.GetString(reader.GetOrdinal("user_password"));
                                    string acity = reader.GetString(reader.GetOrdinal("city"));
                                    string address = reader.GetString(reader.GetOrdinal("address"));
                                    string fName = reader.GetString(reader.GetOrdinal("f_name"));
                                    string lName = reader.GetString(reader.GetOrdinal("l_name"));
                                    DateTime birthDate = reader.GetDateTime(reader.GetOrdinal("birth_date"));
                                    a = Tuple.Create(userPassword, acity, address, fName, lName, birthDate);
                                }



                                return a;
                            case 5: //ID
                                return reader.GetInt32(reader.GetOrdinal("user_id"));
                            case 6: //
                                specieList.Add(reader.GetString(reader.GetOrdinal("species")));
                                break;
                            case 7:
                                return reader.GetInt32(reader.GetOrdinal("animal_id"));
                            case 8:
                                return reader.GetString(reader.GetOrdinal("city"));
                            case 9:
                                tumtip ilan = new()
                                {
                                    noticeid = reader.GetInt32(0),
                                    aid = reader.GetInt32(1),
                                    notice_date = reader.GetDateTime(2),
                                    sex = reader.GetChar(3),
                                    city = reader.GetString(4),
                                    birth_date = reader.GetDateTime(5),
                                    breed = reader.GetString(6),
                                    user_name = reader.GetString(7),
                                    weight = reader.GetInt32(8),
                                    species = reader.GetString(9),
                                    animal_name = reader.GetString(10),

                                };
                                List<int> c = new();
                                List<string> s = new();
                                Tuple<List<int>, List<string>, DateTime, char, DateTime> b;

                                c.Add(ilan.noticeid);
                                c.Add(ilan.aid);
                                c.Add(ilan.weight);

                                s.Add(ilan.city);
                                s.Add(ilan.breed);
                                s.Add(ilan.user_name);
                                s.Add(ilan.species);
                                s.Add(ilan.animal_name);

                                b = Tuple.Create(c, s, ilan.notice_date, ilan.sex, ilan.birth_date);
                                ilanList.Add(b);
                                break;

                            case 10:
                                if (!reader.IsDBNull(reader.GetOrdinal("city")))
                                {
                                    allCities.Add(reader.GetString(reader.GetOrdinal("city")));
                                }
                                break;
                            case 11:
                                allSpecies.Add(reader.GetString(reader.GetOrdinal("species")));
                                break;
                            case 12:
                                Tuple<int, DateTime, string, int, char, string, DateTime, Tuple<string>> tuple = Tuple.Create(
                                reader.GetInt32(reader.GetOrdinal("noticeid")),
                                reader.GetDateTime(reader.GetOrdinal("notice_date")),
                                reader.GetString(reader.GetOrdinal("animal_name")),
                                reader.GetInt32(reader.GetOrdinal("weight")),
                                reader.GetChar(reader.GetOrdinal("sex")),
                                reader.GetString(reader.GetOrdinal("city")),
                                reader.GetDateTime(reader.GetOrdinal("birthdate")),
                                reader.GetString(reader.GetOrdinal("breed")));
                                allAnimals.Add(tuple);
                                break;
                            case 13:
                                Tuple<string, string, string, string> applicant = Tuple.Create(
                                reader.GetString(reader.GetOrdinal("user_city")),
                                reader.GetString(reader.GetOrdinal("user_fname")),
                                reader.GetString(reader.GetOrdinal("user_lname")),
                                reader.GetString(reader.GetOrdinal("user_name")));
                                allApplicants.Add(applicant);
                                break;
                            case 14:
                                Tuple<int, string, int, int, DateTime, string, int> product = Tuple.Create(
                                     reader.GetInt32(0),
                                    reader.GetString(1),
                                    reader.GetInt32(2),
                                    reader.GetInt32(3),
                                    reader.GetDateTime(4),
                                    reader.GetString(5),
                                    reader.GetInt32(6)
                                    );
                                allProducts.Add(product);
                                break;
                            default:
                                break;
                        }
                    }
                    if (todo == 6)
                        return specieList;
                    else if (todo == 9)
                        return ilanList;
                    else if (todo == 10)
                        return allCities;
                    else if (todo == 11)
                        return allSpecies;
                    else if (todo == 12)
                        return allAnimals;
                    else if (todo == 13)
                        return allApplicants;
                    else if (todo == 14)
                        return allProducts;

                }
            }
            return false;
        }
        catch (Exception ex)
        {
            ErrorBox.SetActive(true);
            ErrorBox.transform.Find("Text").GetComponent<TextMeshProUGUI>().SetText(ex.Message);
            Debug.LogError($"Error executing query: {ex.Message}");
            return null;
        }
    }
    class tumtip
    {
        public int noticeid;
        public int aid;
        public DateTime notice_date;
        public char sex;
        public string city;
        public DateTime birth_date;
        public string breed;
        public string user_name;
        public int weight;
        public string species;
        public string animal_name;
    }
}

