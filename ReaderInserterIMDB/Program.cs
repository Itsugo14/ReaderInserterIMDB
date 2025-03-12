using Microsoft.Data.SqlClient;
using ReaderInserterIMDB;
using System.Diagnostics;

using (SqlConnection sqlConn = new SqlConnection("server=MADSLAPTOP;Database=IMDB;" +
               "Integrated security=True;TrustServerCertificate=True"))
{
    sqlConn.Open();
    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();

    SqlTransaction myTrans = sqlConn.BeginTransaction();
    int counter = 0;

    try
    {
        //IInserter inserter = new NormalInserter(sqlConn, myTrans);
        IInserter inserter = new PreparedInserter(sqlConn, myTrans);
        foreach (string line in File.ReadLines("C:/temp/title.basics (1).tsv").Skip(1).Take(10000))
        {
            counter++;
            string[] splitLine = line.Split("\t");
            if (splitLine.Length != 9)
            {
                Console.WriteLine("Linjen har ikke 9 kolonner: " + line);
            }
            else
            {
                string titleType = splitLine[1];
                SqlCommand sqlComm = new SqlCommand("EXEC [dbo].[TitleTypeGetInsertID] @NewTiteType = '"
                    + titleType + "'", sqlConn, myTrans);
                SqlDataReader reader = sqlComm.ExecuteReader();
                if (reader.Read())
                {
                    string titleTypeId = reader["Id"].ToString();
                    //Console.WriteLine("Indlæst Titletypeid: " + titleTypeId);
                    reader.Close();

                    string tconst = splitLine[0];
                    string primaryTitle = splitLine[2].Replace("'", "''");
                    string originalTitle = splitLine[3].Replace("'", "''");
                    string isAdult = CheckBit(splitLine[4]);
                    string startYear = CheckInt(splitLine[5]);
                    string endYear = CheckInt(splitLine[6]);
                    string runtimeMinutes = CheckInt(splitLine[7]);

                    Title newTitle = new Title()
                    {
                        tconst = tconst,
                        titleTypeId = titleTypeId,
                        primaryTitle = primaryTitle,
                        originalTitle = originalTitle,
                        isAdult = isAdult,
                        startYear = startYear,
                        endYear = endYear,
                        runtimeMinutes = runtimeMinutes
                    };
                    inserter.InsertTitle(newTitle);
                }
                else
                {
                    Console.WriteLine("Kunne ikke få id for titletype: " + titleType);
                    reader.Close();
                }
            }
        }
    }
    catch (SqlException ex)
    {
        Console.WriteLine("linje nummer: " + counter + "\r\n" + ex.ToString());
    }
    stopwatch.Stop();
    Console.WriteLine("Elapsed milli: " + stopwatch.ElapsedMilliseconds);

    myTrans.Rollback();
}

string CheckBit(string bit)
{
    if (bit == "1" || bit == "0")
    {
        return bit;
    }
    else if (bit.ToLower() == "\\n")
    {
        return "NULL";
    }
    throw new Exception("Hvordan fanden ser dit bit ud!!!");
}

string CheckInt(string input)
{
    if (int.TryParse(input, out int result))
    {
        return input;
    }
    else if (input.ToLower() == "\\n")
    {
        return "NULL";
    }
    throw new Exception("Hvordan fanden ser din int ud!!!");
}