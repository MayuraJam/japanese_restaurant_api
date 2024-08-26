namespace japanese_resturant_project.model.DatabaseModel
{
    public class Menu_tb
    {
        public Guid menuID { get; set; }
        public string menuName { get; set; }
        public string menuDescription { get; set; }
        public decimal unitPrice { get; set; }
        public string imageURL { get; set; }
        public TimeOnly timeCooking { get; set; }
        public Guid categoryID { get; set; }
        public Guid optionID { get; set; }
        public DateTime createDate { get; set; }
        public DateTime updateDate { get; set; }
        public string categoryName { get; set; }
        public string optionName { get; set; }
        public string value { get; set; }

    }
}
