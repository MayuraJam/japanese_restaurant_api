namespace japanese_resturant_project.model.DatabaseModel
{
    public class Menu_tb
    {
        public Guid menuID { get; set; }
        public string menuName { get; set; }
        public string menuDescription { get; set; }
        public decimal unitPrice { get; set; }
        public int cal { get; set; }
    }
}
