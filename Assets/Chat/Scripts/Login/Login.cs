using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;



public class Login 
{
    public static bool RequestLogin(string userId, string password)
    {
        DataSet data = DatabaseConnect.RequestSelect(
            $"SELECT U_Password FROM account_info WHERE U_UserId = '{userId}'", 
            "account_info");
        string dbPassword = data.Tables[0].Rows[0][0].ToString();
        if (dbPassword == password)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public static bool RequestInsert(string userId, string password)
    {
        return DatabaseConnect.RequestInsert(
            $"INSERT INTO account_info VALUES ('{userId}', '{password}')",
            "account_info");
    }
}
