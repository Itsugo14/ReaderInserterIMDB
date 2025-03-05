using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaderInserterIMDB
{
    public class PreparedInserter : IInserter
    {
        private readonly SqlConnection sqlConn;
        private readonly SqlTransaction myTrans;
        private SqlCommand sqlCommInsert;

        public PreparedInserter(SqlConnection sqlConn, SqlTransaction myTrans)
        {
            this.sqlConn = sqlConn;
            this.myTrans = myTrans;

            sqlCommInsert = new SqlCommand("INSERT INTO [dbo].[Titles]" +
                       "([Tconst],[TitleTypeId],[PrimaryTitle]" +
                       ",[OriginalTitle],[IsAdult],[StartYear],[EndYear],[RuntimeMinutes]) " +
                       "VALUES (@tconst,@titleTypeId,@primaryTitle,@originalTitle," +
                       "@isAdult,@startYear,@endYear,@runtimeMinutes)", sqlConn, myTrans);

            sqlCommInsert.Parameters.Add(CreateParameter("tconst", SqlDbType.VarChar, 10));
            sqlCommInsert.Parameters.Add(CreateParameter("TitleTypeId", SqlDbType.Int));
            sqlCommInsert.Parameters.Add(CreateParameter("PrimaryTitle", SqlDbType.VarChar, -1));
            sqlCommInsert.Parameters.Add(CreateParameter("originalTitle", SqlDbType.VarChar, -1));
            sqlCommInsert.Parameters.Add(CreateParameter("IsAdult", SqlDbType.Bit));
            sqlCommInsert.Parameters.Add(CreateParameter("StartYear", SqlDbType.SmallInt));
            sqlCommInsert.Parameters.Add(CreateParameter("EndYear", SqlDbType.SmallInt));
            sqlCommInsert.Parameters.Add(CreateParameter("RuntimeMinutes", SqlDbType.SmallInt));
            sqlCommInsert.Prepare();
        }

        public void InsertTitle(Title newTitle)
        {
            sqlCommInsert.Parameters["tconst"].Value = newTitle.tconst;
            sqlCommInsert.Parameters["TitleTypeId"].Value = newTitle.titleTypeId;
            sqlCommInsert.Parameters["PrimaryTitle"].Value = newTitle.primaryTitle;
            sqlCommInsert.Parameters["originalTitle"].Value = newTitle.originalTitle;
            sqlCommInsert.Parameters["IsAdult"].Value = CheckBoolNull(newTitle.isAdult);
            sqlCommInsert.Parameters["StartYear"].Value = CheckIntNull(newTitle.startYear);
            sqlCommInsert.Parameters["EndYear"].Value = CheckIntNull(newTitle.endYear);
            sqlCommInsert.Parameters["RuntimeMinutes"].Value = CheckIntNull(newTitle.runtimeMinutes);
            sqlCommInsert.ExecuteNonQuery();
        }

        public static SqlParameter CreateParameter(string parameterName, SqlDbType type, int? size = null)
        {
            SqlParameter result = new SqlParameter(parameterName, type);
            if (size == null)
            {
                result = new SqlParameter(parameterName, type);
            }
            else
            {
                result = new SqlParameter(parameterName, type, (int)size);
            }
            return result;
        }

        public static Object CheckBoolNull(string input)
        {
            if (input == "NULL")
            {
                return DBNull.Value;
            }
            if (input == "1")
            {
                return true;
            }
            else if (input == "0")
            {
                return false;
            }
            throw new Exception("Ingen bool");
        }

        public static Object CheckIntNull(string input)
        {
            if (input == "NULL")
            {
                return DBNull.Value;
            }
            if (int.TryParse(input, out int result))
            {
                return result;
            }
            throw new Exception("Ingen int");
        }
    }
}
