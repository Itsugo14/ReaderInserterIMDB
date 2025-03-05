using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ReaderInserterIMDB
{
    public class NormalInserter : IInserter
    {
        private readonly SqlConnection sqlConn;
        private readonly SqlTransaction myTrans;
        public NormalInserter(SqlConnection sqlConn, SqlTransaction myTrans)
        {
            this.sqlConn = sqlConn;
            this.myTrans = myTrans;
        }

        public void InsertTitle(Title newTitle)
        {
            SqlCommand sqlCommInsert = new SqlCommand("INSERT INTO [dbo].[Titles]" +
                       "([Tconst],[TitleTypeId],[PrimaryTitle]" +
            ",[OriginalTitle],[IsAdult],[StartYear],[EndYear],[RuntimeMinutes])" +
                       "VALUES ('" + newTitle.tconst + "'," + newTitle.titleTypeId +
                       ",'" + newTitle.primaryTitle + "', '" + newTitle.originalTitle +
                       "', " + newTitle.isAdult + "," + newTitle.startYear + "," +
                       newTitle.endYear + "," + newTitle.runtimeMinutes + ")", sqlConn, myTrans);
            sqlCommInsert.ExecuteNonQuery();
        }
    }
}
