namespace japanese_resturant_project.model.DatabaseModel
{
    public class Point_tb
    {
        public string pointID { get; set; }
        public string description { get; set; }
        public decimal? currentPoint { get; set; }
        public DateTime createDate { get; set; }
        public Guid memberID { get; set; }

    }
}
