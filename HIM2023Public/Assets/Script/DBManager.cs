using System.Collections;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using MySql.Data;
using UnityEngine;
using System;
using System.Net;



public class DBManager : MonoBehaviour
{
    private static MySqlConnection sqlconn;
    private MySqlDataReader table;
    private string strconn = "Server=3.36.116.219;Port=3306;Database=him;Uid=root;Pwd=him5;CharSet=utf8;"; 


    public void ListToDebug(List<string[]> result)
    {
        foreach (string[] rows in result)
        {
            string row = "";
            for (int i = 0; i < rows.Length; i++)
            {
                row += rows[i] + " ";
            }
            Debug.Log(row);
        }


}

    public int QueryData(string sql)
    {
        try
        {
            MySqlCommand command = new MySqlCommand();
            command.Connection = sqlconn;
            command.CommandText = sql;
            sqlconn.Open();
            command.ExecuteNonQuery();
            return 1;
        }
        catch (Exception msg)
        {
            Debug.Log(msg);
            return -1;
        }
        finally
        {
            sqlconn.Close();
        }
    }

    public int QueryData(List<string> sql)
    {
        try
        {
            MySqlCommand command = new MySqlCommand();
            command.Connection = sqlconn;
            sqlconn.Open();

            foreach (string _sql in sql)
            {
                command.CommandText = _sql;
                command.ExecuteNonQuery();
            }
            return 1;
        }
        catch (Exception msg)
        {
            Debug.Log(msg);
            return -1;
        }
        finally
        {
            sqlconn.Close();
        }
    }

    public List<string[]> GetData(string sql)
    {
        try
        {
            sqlconn.Open();
            table = new MySqlCommand(sql, sqlconn).ExecuteReader();
            List<string[]> result = new List<string[]>();
            while (table.Read())
            {
                string[] rows = new string[table.FieldCount];
                for (int i = 0; i < table.FieldCount; i++)
                {
                    rows[i] = table[i].ToString();
                }
                result.Add(rows);
            }
            return result;
        }
        catch (Exception msg)
        {
            Debug.Log(msg);
            return null;
        }
        finally
        {
            table.Close();
            sqlconn.Close();
        }
    }





    private void Awake()
    {
        try
        {
            sqlconn = new MySqlConnection(strconn);
            Debug.Log("DB Conn Complete");


        }
        catch(Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

}
