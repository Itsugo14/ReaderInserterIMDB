using Microsoft.Data.SqlClient;

using (SqlConnection sqlConn = new SqlConnection("server=localhost;Database=IMDB;" +
               "Integrated security=True;TrustServerCertificate=True"))
{
    sqlConn.Open();
    foreach (string line in File.ReadLines("C:/temp/title.basics (1).tsv").Skip(1).Take(10000))
    {
        string[] splitLine = line.Split("\t");
        if (splitLine.Length != 9)
        {
            Console.WriteLine("Linjen har ikke 9 kolonner: " + line);
        }
        else
        {
            string titleType = splitLine[1];
            SqlCommand sqlComm = new SqlCommand("EXEC [dbo].[TitleTypeGetInsertID] @NewTiteType = '"
                + titleType + "'", sqlConn);
            SqlDataReader reader = sqlComm.ExecuteReader();
            if (reader.Read())
            {
                int titleTypeId = (int)reader["Id"];
                Console.WriteLine("Indlæst Titletypeid: " + titleTypeId);
                reader.Close();

                string tconst = splitLine[0];
                string primaryTitle = splitLine[2];
                string originalTitle = splitLine[3];
                bool? isAdult = CheckBit(splitLine[4]);
                int? startYear = CheckInt(splitLine[5]);
                int? endYear = CheckInt(splitLine[6]);
                int? runtimeMinutes = CheckInt(splitLine[7]);
            }
            else
            {
                Console.WriteLine("Kunne ikke få id for titletype: " + titleType);
                reader.Close();
            }

        }
    }
}

bool? CheckBit(string bit)
{
    if (bit == "1")
    {
        return true;
    }
    else if (bit == "0")
    {
        return false;
    }
    else if (bit.ToLower() == "\\n")
    {
        return null;
    }
    throw new Exception("Hvordan fanden ser dit bit ud!!!");
}

int? CheckInt(string input)
{
    if (int.TryParse(input, out int result))
    {
        return result;
    }
    else if (input.ToLower() == "\\n")
    {
        return null;
    }
    throw new Exception("Hvordan fanden ser dit bit ud!!!");
}