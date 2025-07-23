using Microsoft.Data.SqlClient;


namespace WebScraping
{
    class AuctionDB
    {
        private static string CONNECTION_STRING = "Data Source=DESKTOP-T625HOC\\SQLEXPRESS;Initial Catalog=WebScraping;Integrated Security=True;Trust Server Certificate=True";
        
        public static int insertIntoAuction(Auction auction)
        {
            SqlConnection conn = new SqlConnection(CONNECTION_STRING);
            conn.Open();
            SqlCommand cmd = new SqlCommand()
            {
                Connection = conn,
                CommandType = System.Data.CommandType.StoredProcedure,
                CommandText = "sp_InsertAuction",
                Parameters =
                {
                    new SqlParameter("Title",auction.Title),
                    new SqlParameter("Description",auction.Description),
                    new SqlParameter("ImageUrl",auction.ImageUrl),
                    new SqlParameter("Link",auction.Link),
                    new SqlParameter("LotCount",auction.LotCount),
                    new SqlParameter("StartDate",auction.StartDate),
                    new SqlParameter("StartMonth",auction.StartMonth),
                    new SqlParameter("StartYear",auction.StartYear),
                    new SqlParameter("StartTime",auction.StartTime),
                    new SqlParameter("EndDate",auction.EndDate),
                    new SqlParameter("EndMonth",auction.EndMonth),
                    new SqlParameter("EndYear",auction.EndYear),
                    new SqlParameter("EndTime",auction.EndTime),
                    new SqlParameter("Location",auction.Location),
                }
            };
            int affectedRow = cmd.ExecuteNonQuery();
            conn.Close();
            return affectedRow;

        }
        public static void truncateAuction()
        {
            SqlConnection conn = new SqlConnection(CONNECTION_STRING);
            conn.Open();
            SqlCommand cmd = new SqlCommand()
            {
                Connection = conn,
                CommandType = System.Data.CommandType.StoredProcedure,
                CommandText = "sp_TruncateAuction"
            };
            cmd.ExecuteNonQuery();
            conn.Close();
        }
    }
}
